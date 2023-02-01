namespace RustyOptions.Tests;

public sealed class UnitTests
{
    [Fact]
    public void CanCreateAndCompare()
    {
        var u1 = Unit.Default;
        Unit u2;

        Assert.True(u1.Equals(u2));
        Assert.True(u1.Equals((object)u2));
        Assert.False(u1.Equals(""));
        Assert.Equal(0, u1.GetHashCode());

        Assert.True(u1 == u2);
        Assert.False(u1 != u2);
        Assert.False(u1 < u2);
        Assert.True(u1 <= u2);
        Assert.False(u1 > u2);
        Assert.True(u1 >= u2);
        Assert.Equal(u1, u1 + u2);

        Assert.Equal(0, u1.CompareTo(u2));
    }

    [Fact]
    public void CanConvertToString()
    {
        var u1 = Unit.Default;

        Assert.Equal("()", u1.ToString());
        Assert.Equal("()", u1.ToString(null, null));

        Span<char> buffer = stackalloc char[10];
        var success = u1.TryFormat(buffer, out int written, ReadOnlySpan<char>.Empty, null);

        Assert.True(success);
        Assert.Equal(2, written);
        Assert.True(buffer[..written].SequenceEqual("()"));

        buffer = Span<char>.Empty;
        success = u1.TryFormat(buffer, out written, ReadOnlySpan<char>.Empty, null);

        Assert.False(success);
        Assert.Equal(0, written);
    }

    [Fact]
    public void CanConvertToValueTuple()
    {
        Unit u1;
        ValueTuple vt1;

        Unit u2 = vt1;
        ValueTuple vt2 = u1;

        Assert.Equal(vt1, vt2);
        Assert.Equal(u1, u2);
    }
}
