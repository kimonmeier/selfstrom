using AutoMapper;
using MyStrom.Api;
using MyStrom.Api.Models.DeviceInfo;
using Quartz;
using SelfStrom.Shared.Data;
using SelfStrom.Shared.Services.Worker;
using SelfStrom.Worker.Data.Repositories;
using SelfStrom.Worker.Extensions;
using Serilog;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace SelfStrom.Worker.Background;

internal class SearchDeviceJob : IJob
{
    private readonly ILogger _logger;
    private readonly AvailableDevicesService.AvailableDevicesServiceClient _availableDevicesServiceClient;
    private readonly ConnectedDevicesRepository _devicesRepository;
    private readonly DbTransactionFactory _transactionFactory;
    private readonly IMapper _mapper;

    public SearchDeviceJob(ILogger logger, AvailableDevicesService.AvailableDevicesServiceClient availableDevicesServiceClient, IMapper mapper, ConnectedDevicesRepository devicesRepository, DbTransactionFactory transactionFactory)
    {
        _logger = logger;
        _availableDevicesServiceClient = availableDevicesServiceClient;
        _mapper = mapper;
        _devicesRepository = devicesRepository;
        _transactionFactory = transactionFactory;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.Debug("Scanning for devices");
        var interfaces = NetworkInterface.GetAllNetworkInterfaces().Where(x => (x.NetworkInterfaceType == NetworkInterfaceType.Ethernet || x.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet || x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && !x.Name.Contains("docker", StringComparison.OrdinalIgnoreCase) && !x.Description.Contains("docker", StringComparison.OrdinalIgnoreCase));
        List<IPAddress> pingableIPAddress = new List<IPAddress>();
        foreach (var iface in interfaces) {
            pingableIPAddress.AddRange(await ScanNetworkInterface(iface));
        }

        _logger.Debug("Sorting out ips on which port 80 isn't open");
        // Remove IP's which didn't open port 80
        for (int i = 0; i < pingableIPAddress.Count; i++)
        {
            IPAddress currentAddress = pingableIPAddress[i];
            if (currentAddress.IsPortOpen(80))
            {
                _logger.Verbose($"Port 80 of IP {currentAddress} is open");
                continue;
            }
            _logger.Verbose($"Port 80 of IP {currentAddress} is not open");

            pingableIPAddress.Remove(currentAddress);
            i--;
        }

        ReportAvailableDevicesMessage availableDevicesMessage = new ReportAvailableDevicesMessage();
        _logger.Debug("Finding MyStrom Devices");
        foreach (IPAddress address in pingableIPAddress)
        {
            _logger.Debug("Search for Device info on IP: {0}", address);
            DeviceApi deviceApi = new($"http://{address}");
            try
            {
                DeviceInfoResponse deviceInfoResponse = await deviceApi.GetDeviceInfoAsync();
                _logger.Debug($"Found Device {deviceInfoResponse.Type} Information on IP: {deviceInfoResponse.IPAddress}");

                if (!IsSwitch(deviceInfoResponse))
                {
                    _logger.Debug("The Device found isn't a switch so it can't be accessed");
                    continue;
                }

                availableDevicesMessage.Devices.Add(_mapper.Map<AvailableDevice>(deviceInfoResponse));
            } catch (Exception ex)
            {
                _logger.Verbose(ex, "Exception occured during device api");
                continue;
            }            
        }

        // Refresh lokal IPAddresses so that the IP's are up to date
        using (var transaction = _transactionFactory.CreateTransaction())
        {
            foreach (var device in availableDevicesMessage.Devices)
            {
                var connectedDevice = await _devicesRepository.FindByMacAddress(device.MacAddress);

                if (connectedDevice is null)
                {
                    continue;
                }

                connectedDevice.IPAddress = device.IpAddress;
            }

            await transaction.Commit(context.CancellationToken);
        }

        try
        {
            _logger.Debug("Found {0} available Devices", availableDevicesMessage.Devices.Count);
            await _availableDevicesServiceClient.ReportAvailableDevicesAsync(availableDevicesMessage);
        } catch (Exception e)
        {
            JobExecutionException e2 = new JobExecutionException(e);
            e2.RefireImmediately = true;
            throw e2;
        }
    }

    private static bool IsSwitch(DeviceInfoResponse deviceInfoResponse) => deviceInfoResponse.Type switch
    {
        MyStrom.Api.Models.DeviceType.Switch_CH_V1 or MyStrom.Api.Models.DeviceType.Switch_CH_V2 or MyStrom.Api.Models.DeviceType.Switch_EU => true,
        _ => false,
    };

    private async Task<List<IPAddress>> ScanNetworkInterface(NetworkInterface networkInterface)
    {
        List<UnicastIPAddressInformation> unicastAddresses = networkInterface.GetIPProperties().UnicastAddresses.ToList();
        var ipv4Addresses = unicastAddresses.Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork).ToList();
        if (!ipv4Addresses.Any())
        {
            return new List<IPAddress>();
        }

        List<IPAddress> pingableDevice = new List<IPAddress>();

        foreach (UnicastIPAddressInformation ipAddressInformation in ipv4Addresses)
        {
            _logger.Debug("Scanning IP: {0} with Submask {1} started", ipAddressInformation.Address, ipAddressInformation.IPv4Mask);
            pingableDevice.AddRange(await ScanSubnet(ipAddressInformation.Address, ipAddressInformation.IPv4Mask));
            _logger.Debug("Scanning IP: {0} with Submask {1} finished", ipAddressInformation.Address, ipAddressInformation.IPv4Mask);
        }

        return pingableDevice;
    }

    private async Task<List<IPAddress>> ScanSubnet(IPAddress address, IPAddress subnetMask)
    {
        List<IPAddress> pingableDevice = new List<IPAddress>();

        _logger.Debug("Scanning subnet {0} with Submask {1}", address, subnetMask);
        await Parallel.ForEachAsync(GetSubnetRange(address, subnetMask),
            new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            },
            async (ipAdress, _) =>
            {
                using Ping ping = new();

                var reply = await ping.SendPingAsync(ipAdress, 100);
                if (reply.Status != IPStatus.Success)
                {
                    _logger.Verbose($"Ping failed for {ipAdress}");
                    return;
                }
                _logger.Verbose($"Ping success for {ipAdress}");

                pingableDevice.Add(ipAdress);
            });

        _logger.Debug("Found {0} pingable IP's in subnet", pingableDevice.Count);
        return pingableDevice;
    }

    private IPAddress[] GetSubnetRange(IPAddress address, IPAddress subnetMask)
    {
        var addressBytes = address.GetAddressBytes();
        var maskBytes = subnetMask.GetAddressBytes();

        var startIPBytes = new byte[addressBytes.Length];
        var endIPBytes = new byte[addressBytes.Length];

        for (int i = 0; i < addressBytes.Length; i++)
        {
            startIPBytes[i] = (byte)(addressBytes[i] & maskBytes[i]);
            endIPBytes[i] = (byte)(startIPBytes[i] | ~maskBytes[i]);
        }

        var startIP = new IPAddress(startIPBytes);
        var endIP = new IPAddress(endIPBytes);

        var start = BitConverter.ToUInt32(startIP.GetAddressBytes().Reverse().ToArray(), 0);
        var end = BitConverter.ToUInt32(endIP.GetAddressBytes().Reverse().ToArray(), 0);

        // Skip network address (first IP) and broadcast address (last IP)
        var usableStart = start + 1;
        var usableEnd = end - 1;

        // Handle edge case where subnet has no usable hosts (e.g., /31 or /32)
        if (usableStart > usableEnd)
        {
            _logger.Debug("Subnet has no usable host addresses");
            return Array.Empty<IPAddress>();
        }

        var addresses = new IPAddress[usableEnd - usableStart + 1];
        for (uint i = 0; i <= usableEnd - usableStart; i++)
        {
            addresses[i] = new IPAddress(BitConverter.GetBytes(usableStart + i).Reverse().ToArray());
        }
        
        _logger.Debug("Found {0} IP's in subnet", addresses.Length);
        _logger.Debug("First IP: {0}", addresses[0]);
        _logger.Debug("Last IP: {0}", addresses[addresses.Length - 1]);
        _logger.Debug("Subnet Mask: {0}", subnetMask);       

        return addresses;
    }
}
