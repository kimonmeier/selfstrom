using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Server.Helper;
using SelfStrom.Server.Models;
using SelfStrom.Server.Repositories;
using SelfStrom.Shared.Services.Worker;

namespace SelfStrom.Server.Services.Workers;

public class AvailableDeviceService : SelfStrom.Shared.Services.Worker.AvailableDevicesService.AvailableDevicesServiceBase
{
    private readonly DeviceService _deviceService;
    private readonly WorkerRepository _workerRepository;
    private readonly IMapper _mapper;

    public AvailableDeviceService(DeviceService deviceService, IMapper mapper, WorkerRepository workerRepository)
    {
        _deviceService = deviceService;
        _mapper = mapper;
        _workerRepository = workerRepository;
    }

    public override async Task<Empty> ReportAvailableDevices(ReportAvailableDevicesMessage request, ServerCallContext context)
    {
        Worker? worker = await context.GetWorkerAsync(_workerRepository);

        if (worker is null)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, "The provided api key is wrong!"));
        }

        _deviceService.ReportAvailableDevices(worker.HomeId, _mapper.Map<List<DeviceModel>>(request.Devices.ToList()));

        return new Empty();
    }
}
