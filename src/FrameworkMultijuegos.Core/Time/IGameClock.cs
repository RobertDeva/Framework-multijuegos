namespace FrameworkMultijuegos.Core.Time;

/// <summary>Provides logical simulation time without exposing wall-clock APIs to domain rules.</summary>
public interface IGameClock
{
    SimulationInstant Now { get; }
}
