using SelfStrom.Shared.Data.Entities;

namespace SelfStrom.Server.Data.Entities;

public class Device : IPersistenEntity
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }

    public Room Room { get; set; } = null!;

    public string MacAddress { get; set; } = null!;

    public DateTime? LastSeen { get; set; }

    public string? Description { get; set; }
    
    public bool IsDeleted { get; set; }
}
