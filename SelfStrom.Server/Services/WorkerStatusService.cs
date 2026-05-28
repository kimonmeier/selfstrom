namespace SelfStrom.Server.Services;

public class WorkerStatusService
{
    // 2 Minutes grace period before Worker is offline
    private const int WorkerGracePeriodInSeconds = -1 * 60 * 2;

    public Dictionary<Guid, DateTime> lastPingByWorker = new Dictionary<Guid, DateTime>();

    public void WorkerPing(Guid workerId)
    {
        if (!lastPingByWorker.ContainsKey(workerId))
        {
            lastPingByWorker.Add(workerId, DateTime.Now);
            return;
        }

        lastPingByWorker[workerId] = DateTime.Now;
    }

    public bool IsWorkerRunning(Guid workerId)
    {
        if (!lastPingByWorker.ContainsKey(workerId)) { 
            return false;
        }

        DateTime lastPing = lastPingByWorker[workerId];

        return lastPing > DateTime.Now.AddSeconds(WorkerGracePeriodInSeconds);
    }
}
