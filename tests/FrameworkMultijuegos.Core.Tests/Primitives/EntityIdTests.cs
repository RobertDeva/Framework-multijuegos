using FrameworkMultijuegos.Core.Primitives;

namespace FrameworkMultijuegos.Core.Tests.Primitives;

public sealed class EntityIdTests
{
    [Fact]
    public void New_CreatesNonEmptyDistinctIds()
    {
        var first = EntityId.New();
        var second = EntityId.New();

        Assert.False(first.IsEmpty);
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Constructor_RejectsEmptyGuid()
    {
        Assert.Throws<ArgumentException>(() => new EntityId(Guid.Empty));
    }
}
