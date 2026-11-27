using FrameworkMultijuegos.Core.Primitives;

namespace FrameworkMultijuegos.Core.Entities;

/// <summary>
/// Base for entities with stable identity and a minimal simulation lifecycle.
/// Removal only stops simulation participation; it does not represent a gameplay outcome.
/// </summary>
public abstract class EntityBase : IEquatable<EntityBase>
{
    protected EntityBase(EntityId id)
    {
        if (id.IsEmpty)
            throw new ArgumentException("An entity must have a non-empty identifier.", nameof(id));

        Id = id;
    }

    public EntityId Id { get; }
    public EntityLifecycleState LifecycleState { get; private set; } = EntityLifecycleState.Active;
    public bool IsActive => LifecycleState == EntityLifecycleState.Active;

    /// <summary>Removes the entity from simulation participation. Repeated calls are harmless.</summary>
    public bool Remove()
    {
        if (!IsActive)
            return false;

        LifecycleState = EntityLifecycleState.Removed;
        return true;
    }

    public bool Equals(EntityBase? other) => other is not null && Id == other.Id;

    public override bool Equals(object? obj) => obj is EntityBase other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(EntityBase? left, EntityBase? right) => Equals(left, right);

    public static bool operator !=(EntityBase? left, EntityBase? right) => !Equals(left, right);
}
