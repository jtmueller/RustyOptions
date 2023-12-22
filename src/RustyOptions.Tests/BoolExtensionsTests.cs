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
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
