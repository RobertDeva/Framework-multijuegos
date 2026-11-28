using FrameworkMultijuegos.Core.Time;

namespace FrameworkMultijuegos.Core.Tests.Time;

public sealed class SimulationTimeTests
{
    [Fact]
    public void ManualClock_AdvancesOnlyWhenRequested()
    {
        var clock = new ManualGameClock();

        clock.Advance(SimulationDuration.FromTicks(5));

        Assert.Equal(5, clock.Now.Tick);
    }

    [Fact]
    public void Duration_RejectsNegativeTicks()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SimulationDuration.FromTicks(-1));
    }

    [Fact]
    public void Instant_RejectsElapsedTimeFromFuture()
    {
        var now = new SimulationInstant(3);
        Assert.Throws<ArgumentException>(() => now.ElapsedSince(new SimulationInstant(4)));
    }
}
