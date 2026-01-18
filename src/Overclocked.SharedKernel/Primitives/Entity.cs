namespace Overclocked.SharedKernel.Primitives;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    protected Entity() { }

    protected Entity(TId id) => Id = id;

    public TId Id { get; private init; } = default!;

    public override bool Equals(object? obj) => obj is Entity<TId> entity && Id.Equals(entity.Id);

    public bool Equals(Entity<TId>? other) => Equals((object?)other);

    public static bool operator ==(Entity<TId> left, Entity<TId> right) => Equals(left, right);

    public static bool operator !=(Entity<TId> left, Entity<TId> right) => !Equals(left, right);

    public override int GetHashCode() => Id.GetHashCode();
}
