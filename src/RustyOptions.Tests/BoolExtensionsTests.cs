using RustyOptions.Async;

namespace RustyOptions.Tests;

#pragma warning disable CA1707 // Identifiers should not contain underscores

public class BoolExtensionsTests
{
    [Fact]
    public void Then_ReturnsOptionWithValue_WhenBooleanIsTrue()
    {
        // Arrange
        bool condition = true;

        // Act
        var result = condition.Then(() => 42);

        // Assert
        Assert.True(result.IsSome(out int resultVal));
        Assert.Equal(42, resultVal);
    }

    [Fact]
    public void Then_ReturnsDefaultOption_WhenBooleanIsFalse()
    {
        // Arrange
        bool condition = false;

        // Act
        var result = condition.Then(() => 42);

        // Assert
        Assert.True(result.IsNone);
    }

    [Fact]
    public void ThenSome_ReturnsOptionWithValue_WhenBooleanIsTrue()
    {
        // Arrange
        bool condition = true;
        int value = 42;

        // Act
        var result = condition.ThenSome(value);

        // Assert
        Assert.True(result.IsSome(out int resultVal));
        Assert.Equal(42, resultVal);
    }

    [Fact]
    public void ThenSome_ReturnsDefaultOption_WhenBooleanIsFalse()
    {
        // Arrange
        bool condition = false;
        int value = 42;

        // Act
        var result = condition.ThenSome(value);

        // Assert
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task ThenAsync_Task_ReturnsDefaultWhenFalse()
    {
        bool condition = false;
        var result = await condition.ThenAsync(() => Task.FromResult("Test"));
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task ThenAsync_Task_ReturnsResultWhenTrue()
    {
        bool condition = true;
        var result = await condition.ThenAsync(() => Task.FromResult("Test"));
        Assert.True(result.IsSome(out var resultVal));
        Assert.Equal("Test", resultVal);
    }

    [Fact]
    public async Task ThenAsync_ValueTask_ReturnsDefaultWhenFalse()
    {
        bool condition = false;
        var result = await condition.ThenAsync(() => new ValueTask<string>("Test"));
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task ThenAsync_ValueTask_ReturnsResultWhenTrue()
    {
        bool condition = true;
        var result = await condition.ThenAsync(() => new ValueTask<int>(42));
        Assert.True(result.IsSome(out var resultVal));
        Assert.Equal(42, resultVal);
    }

    [Fact]
    public async Task ThenSomeAsync_Task_WhenTrue_ShouldReturnOptionWithValue()
    {
        bool condition = true;
        var result = await condition.ThenSomeAsync(Task.FromResult("Hello"));

        Assert.True(result.IsSome(out var value));
        Assert.Equal("Hello", value);
    }

    [Fact]
    public async Task ThenSomeAsync_Task_NotCompleted_WhenTrue_ShouldReturnOptionWithValue()
    {
        bool condition = true;
        var result = await condition.ThenSomeAsync(GetTask("Hello"));

        Assert.True(result.IsSome(out var value));
        Assert.Equal("Hello", value);
    }

    [Fact]
    public async Task ThenSomeAsync_Task_WhenFalse_ShouldReturnDefaultOption()
    {
        bool condition = false;
        var result = await condition.ThenSomeAsync(Task.FromResult("Hello"));

        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task ThenSomeAsync_ValueTask_WhenTrue_ShouldReturnOptionWithValue()
    {
        bool condition = true;
        var result = await condition.ThenSomeAsync(new ValueTask<string>("Hello"));

        Assert.True(result.IsSome(out var value));
        Assert.Equal("Hello", value);
    }

    [Fact]
    public async Task ThenSomeAsync_ValueTask_NotCompleted_WhenTrue_ShouldReturnOptionWithValue()
    {
        bool condition = true;
        var result = await condition.ThenSomeAsync(GetValueTask("Hello"));

        Assert.True(result.IsSome(out var value));
        Assert.Equal("Hello", value);
    }

    [Fact]
    public async Task ThenSomeAsync_ValueTask_WhenFalse_ShouldReturnDefaultOption()
    {
        bool condition = false;
        var result = await condition.ThenSomeAsync(new ValueTask<string>("Hello"));

        Assert.True(result.IsNone);
    }

    private static async Task<T> GetTask<T>(T value)
    {
        await Task.Yield();
        return value;
    }

    private static async ValueTask<T> GetValueTask<T>(T value)
    {
        await Task.Yield();
        return value;
    }
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
