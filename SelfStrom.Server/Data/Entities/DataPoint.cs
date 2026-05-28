using SelfStrom.Shared.Data.Entities;

namespace SelfStrom.Server.Data.Entities;

public class DataPoint : IEntity
{
    public Guid Id { get; set; }

    public Guid DeviceId { get; set; }

    public Device Device { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public double Value { get; set; }
}
