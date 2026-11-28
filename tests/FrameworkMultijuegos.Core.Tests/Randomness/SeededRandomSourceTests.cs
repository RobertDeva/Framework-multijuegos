using FrameworkMultijuegos.Core.Randomness;

namespace FrameworkMultijuegos.Core.Tests.Randomness;

public sealed class SeededRandomSourceTests
{
    [Fact]
    public void EqualSeeds_ProduceEqualSequences()
    {
        var first = new SeededRandomSource(12345);
        var second = new SeededRandomSource(12345);

        for (var index = 0; index < 100; index++)
            Assert.Equal(first.NextInt(-10, 50), second.NextInt(-10, 50));
    }

    [Fact]
    public void NextInt_StaysWithinHalfOpenRange()
    {
        var random = new SeededRandomSource(7);

        for (var index = 0; index < 1000; index++)
        {
            var value = random.NextInt(2, 9);
            Assert.InRange(value, 2, 8);
        }
    }

    [Fact]
    public void NextInt_RejectsEmptyRange()
    {
        var random = new SeededRandomSource(1);
        Assert.Throws<ArgumentOutOfRangeException>(() => random.NextInt(3, 3));
    }
}
