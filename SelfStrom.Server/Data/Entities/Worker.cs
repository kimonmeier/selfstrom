using SelfStrom.Shared.Data.Entities;

namespace SelfStrom.Server.Data.Entities;

public class Worker : IEntity
{
    public Guid Id { get; set; }

    public int Number { get; set; }

    public string WorkerName { get; set; } = null!;

    public Guid HomeId { get; set; }

    public Home Home { get; set; } = null!;

    public string ApiKey { get; set; } = null!;
}
