using SelfStrom.Shared.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Data.Entities;

internal class DataPoint : IEntity
{
    public Guid Id { get; set; }

    public Guid DeviceId { get; set; }

    public ConnectedDevices Device { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public double Value { get; set; }

    public bool IsSynced { get; set; }
}
