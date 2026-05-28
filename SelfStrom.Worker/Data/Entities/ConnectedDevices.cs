using SelfStrom.Shared.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Data.Entities;

internal class ConnectedDevices : IEntity
{
    public Guid Id { get; set; }

    public string MacAdress { get; set; } = null!;

    public string? IPAddress { get; set; }
}
