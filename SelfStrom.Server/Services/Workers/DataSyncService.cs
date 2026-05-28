using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Server.Helper;
using SelfStrom.Server.Repositories;
using SelfStrom.Shared.Data;
using SelfStrom.Shared.Services.Worker;

namespace SelfStrom.Server.Services.Workers;

public class DataSyncService : SelfStrom.Shared.Services.Worker.DataSyncService.DataSyncServiceBase
{
    private readonly DeviceService _deviceService;
    private readonly WorkerRepository _workerRepository;
    private readonly DataPointRepository _datapointRepository;
    private readonly DbTransactionFactory _dbTransactionFactory;
    private readonly IMapper _mapper;

    public DataSyncService(DeviceService deviceService, WorkerRepository workerRepository, IMapper mapper, DbTransactionFactory dbTransactionFactory, DataPointRepository datapointRepository)
    {
        _deviceService = deviceService;
        _workerRepository = workerRepository;
        _mapper = mapper;
        _dbTransactionFactory = dbTransactionFactory;
        _datapointRepository = datapointRepository;
    }

    public override async Task<ListConnectedDevicesResponse> ListConnectedDevices(Empty request, ServerCallContext context)
    {
        Worker? worker = await context.GetWorkerAsync(_workerRepository);

        if (worker is null)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, "The provided api key is wrong!"));
        }

        var devices = _deviceService.GetConnectedDevicesByHome(worker.HomeId);

        ListConnectedDevicesResponse response = new ListConnectedDevicesResponse();
        response.Devices.AddRange(_mapper.Map<List<ConnectedDeviceProto>>(devices));

        return response;
    }

    public override async Task<Empty> ReportDataPoints(ReportDataPointsRequest request, ServerCallContext context)
    {
        Worker? worker = await context.GetWorkerAsync(_workerRepository);

        if (worker is null)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, "The provided api key is wrong!"));
        }

        using (var transaction = _dbTransactionFactory.CreateTransaction()) {
            foreach (var dataPointGroup in request.DataPoints.GroupBy(x => x.DeviceId))
            {
                if (!await _workerRepository.IsDeviceAssignedToWorker(Guid.Parse(dataPointGroup.Key), worker.Id)) {
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, "The provided device is not assigned to the worker!"));
                }

                foreach (var dataPoint in dataPointGroup)
                {
                    await _datapointRepository.AddAsync(new DataPoint()
                    {
                        DeviceId = Guid.Parse(dataPoint.DeviceId),
                        Value = dataPoint.Value,
                        Timestamp = dataPoint.Timestamp.ToDateTime(),
                    });
                }
            }

            await transaction.Commit(context.CancellationToken);
        }

        return new Empty();
    }
}
