using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Server.Helper;
using SelfStrom.Server.Repositories;
using SelfStrom.Shared.Data;
using SelfStrom.Shared.Services.Website;

namespace SelfStrom.Server.Services.Website;

internal class WebsiteDeviceService : SelfStrom.Shared.Services.Website.DeviceService.DeviceServiceBase
{
    private readonly DeviceService _deviceService;
    private readonly DeviceRepository _deviceRepository;
    private readonly DbTransactionFactory _transactionFactory;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public WebsiteDeviceService(DeviceRepository deviceRepository, UserManager<ApplicationUser> userManager, IMapper mapper, DbTransactionFactory transactionFactory, DeviceService deviceService)
    {
        _deviceRepository = deviceRepository;
        _userManager = userManager;
        _mapper = mapper;
        _transactionFactory = transactionFactory;
        _deviceService = deviceService;
    }

    public override async Task<GetDevicesResponse> GetDevices(GetDevicesRequest request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new GetDevicesResponse();
        }

        List<Device> devices = await _deviceRepository.GetDevicesByRoomAsync(Guid.Parse(request.RoomId));

        GetDevicesResponse response = new GetDevicesResponse();
        response.Devices.AddRange(_mapper.Map<List<DeviceProto>>(devices));

        return response;
    }

    public override async Task<GetAvailableDevicesResponse> GetAvailableDevices(GetAvailableDevicesRequest request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new GetAvailableDevicesResponse();
        }

        var availableDevices = _mapper.Map<List<AvailableDeviceProto>>(_deviceService.GetAvailableDevicesByHome(Guid.Parse(request.HomeId)));

        GetAvailableDevicesResponse response = new GetAvailableDevicesResponse();
        response.Devices.AddRange(availableDevices);

        return response;
    }

    public override async Task<Empty> CreateDevice(CreateDeviceRequest request, ServerCallContext context)
    {
        ApplicationUser? user = await context.GetAuthenticatedUser(_userManager);

        if (user == null)
        {
            return new Empty();
        }

        using (var dbTransaction = _transactionFactory.CreateTransaction())
        {
            var availableDevice = _deviceService.GetAvailableDevicesByHome(Guid.Parse(request.HomeId)).Single(x => x.Id.Equals(Guid.Parse(request.DeviceId)));
            Device device = new()
            {
                Id = Guid.Parse(request.DeviceId),
                RoomId = Guid.Parse(request.RoomId),
                MacAddress = availableDevice.MacAddress,
                IsDeleted = false
            };

            if (request.HasDescription)
            {
                device.Description = request.Description;
            }

            _deviceService.DeviceCreated(Guid.Parse(request.HomeId), device);

            await _deviceRepository.AddAsync(device);
            await dbTransaction.Commit(context.CancellationToken);
        }
        using DbTransaction transaction = _transactionFactory.CreateTransaction();
        await transaction.Commit(context.CancellationToken);
        
        return new Empty();
    }
}
