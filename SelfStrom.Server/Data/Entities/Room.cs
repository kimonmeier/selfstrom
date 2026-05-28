using SelfStrom.Shared.Data.Entities;

namespace SelfStrom.Server.Data.Entities;

public class Room : IPersistenEntity
{
    public Guid Id { get; set; }

    public Guid HomeId { get; set; }

    public int Number {  get; set; }

    public string Name { get; set; } = null!;

    public Home Home { get; set; } = null!;

    public bool IsDeleted { get; set; }
}
