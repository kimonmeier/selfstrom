using AutoMapper;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Server.Models;
using SelfStrom.Server.Repositories;
using System.Threading.Tasks;

namespace SelfStrom.Server.Services;

public class DeviceService
{
    private Dictionary<Guid, List<DeviceModel>> AvailableDevices { get; set; }

    private Dictionary<Guid, List<DeviceModel>> Devices { get; set; }


    private readonly IMapper _mapper;

    public DeviceService(IMapper mapper, IServiceProvider provider)
    {
        _mapper = mapper;
        AvailableDevices = new Dictionary<Guid, List<DeviceModel>>();
        Devices = new Dictionary<Guid, List<DeviceModel>>();


        using (IServiceScope scope = provider.CreateScope())
        {
            var deviceRepository = scope.ServiceProvider.GetRequiredService<DeviceRepository>();

            // Load existing Devices
            var devices = deviceRepository.ListAllDevices().ConfigureAwait(true).GetAwaiter().GetResult();
            foreach (var device in devices)
            {
                DeviceCreated(device.Room.HomeId, device);
            }
        }
    }

    public void ReportAvailableDevices(Guid homeId, List<DeviceModel> deviceModels)
    {
        if (!this.AvailableDevices.ContainsKey(homeId))
        {
            this.AvailableDevices.Add(homeId, new List<DeviceModel>());
        }

        var devices = Devices.GetValueOrDefault(homeId) ?? new List<DeviceModel>();

        var modelsToAdd = deviceModels.Where(avDev => !devices.Any(dev => avDev.MacAddress == dev.MacAddress));

        this.AvailableDevices[homeId].Clear();
        foreach (var model in modelsToAdd) {
            model.Id = Guid.NewGuid();

            this.AvailableDevices[homeId].Add(model);
        }
    }

    public void DeviceCreated(Guid homeId, Device device)
    {
        if (!this.Devices.ContainsKey(homeId))
        {
            this.Devices.Add(homeId, new List<DeviceModel>());
        }

        if (!this.AvailableDevices.ContainsKey(homeId))
        {
            this.AvailableDevices.Add(homeId, new List<DeviceModel>());
        }

        this.AvailableDevices[homeId].RemoveAll(x => x.MacAddress == device.MacAddress);

        this.Devices[homeId].Add(_mapper.Map<DeviceModel>(device));
    }

    public void DeviceDeleted(Guid homeId, Device device)
    {
        if (!this.Devices.ContainsKey(homeId))
        {
            this.Devices.Add(homeId, new List<DeviceModel>());
        }

        this.Devices[homeId].RemoveAll(x => x.Id == device.Id);
    }

    public List<DeviceModel> GetAvailableDevicesByHome(Guid homeId)
    {
        return AvailableDevices.GetValueOrDefault(homeId) ?? new List<DeviceModel>();
    }

    public List<DeviceModel> GetConnectedDevicesByHome(Guid homeId)
    {
        return Devices.GetValueOrDefault(homeId) ?? new List<DeviceModel>();
    }
}
