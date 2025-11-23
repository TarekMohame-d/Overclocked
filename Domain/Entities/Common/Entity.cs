namespace Domain.Entities.Common;

public abstract class Entity
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; set; }
}
