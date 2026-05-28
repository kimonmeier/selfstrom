using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Server.Helper;
using SelfStrom.Server.Repositories;

namespace SelfStrom.Server.Services.Workers;

public class PingService : SelfStrom.Shared.Services.Worker.PingService.PingServiceBase
{
    private readonly WorkerStatusService _statusService;
    private readonly WorkerRepository _workerRepository;

    public PingService(WorkerStatusService statusService, WorkerRepository workerRepository)
    {
        _statusService = statusService;
        _workerRepository = workerRepository;
    }

    public async override Task<Empty> SendPing(Empty request, ServerCallContext context)
    {
        Worker? worker = await context.GetWorkerAsync(_workerRepository);

        if (worker is null)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, "The provided api key is wrong!"));
        }

        _statusService.WorkerPing(worker.Id);

        return new Empty();
    }
}
