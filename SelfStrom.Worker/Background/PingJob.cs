using Google.Protobuf.WellKnownTypes;
using Quartz;
using SelfStrom.Shared.Services.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Background;

internal class PingJob : IJob
{
    private readonly PingService.PingServiceClient _pingServiceClient;

    public PingJob(PingService.PingServiceClient pingServiceClient)
    {
        _pingServiceClient = pingServiceClient;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _pingServiceClient.SendPingAsync(new Empty());
    }
}
