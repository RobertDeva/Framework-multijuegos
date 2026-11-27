namespace FrameworkMultijuegos.Core.Entities;

/// <summary>
/// Technical participation state of an entity in a simulation.
/// Gameplay states such as dead, defeated, stunned or destroyed belong to game rules.
/// </summary>
public enum EntityLifecycleState
{
    Active = 0,
    Removed = 1
}
