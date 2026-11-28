namespace FrameworkMultijuegos.Core.Time;

/// <summary>Advances simulation time explicitly and never reads the machine clock.</summary>
public sealed class ManualGameClock : IGameClock
{
    public SimulationInstant Now { get; private set; } = SimulationInstant.Zero;

    public void Advance(SimulationDuration duration) => Now = Now.Add(duration);

    public void Reset() => Now = SimulationInstant.Zero;
}
