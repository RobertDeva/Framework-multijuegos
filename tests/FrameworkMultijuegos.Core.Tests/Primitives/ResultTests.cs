using FrameworkMultijuegos.Core.Primitives;

namespace FrameworkMultijuegos.Core.Tests.Primitives;

public sealed class ResultTests
{
    [Fact]
    public void Success_ExposesValueWithoutError()
    {
        var result = Result<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_ExposesErrorAndRejectsValueAccess()
    {
        var error = new GameError("invalid", "Operation is invalid.");
        var result = Result<int>.Failure(error);

        Assert.True(result.IsFailure);
        Assert.Same(error, result.Error);
        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }

    [Fact]
    public void GameError_RejectsBlankFields()
    {
        Assert.Throws<ArgumentException>(() => new GameError(" ", "message"));
        Assert.Throws<ArgumentException>(() => new GameError("code", " "));
    }
}
