using SelfStrom.Shared.Data.Entities;

namespace SelfStrom.Server.Data.Entities;

public class Home : IPersistenEntity
{
    public Guid Id { get; set; }

    public int Number { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string UserId { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;

    public bool IsDeleted { get; set; }
}
