using AutoMapper;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Server.Services;
using SelfStrom.Shared.Services.Website;

namespace SelfStrom.Server.Mappings.Workers;

public class WorkerStatusResolver : IValueResolver<Worker, WorkerProto, bool>
{
    private readonly WorkerStatusService _workerStatusService;

    public WorkerStatusResolver(WorkerStatusService workerStatusService)
    {
        _workerStatusService = workerStatusService;
    }

    public bool Resolve(Worker source, WorkerProto destination, bool destMember, ResolutionContext context)
    {
        return _workerStatusService.IsWorkerRunning(source.Id);
    }
}
