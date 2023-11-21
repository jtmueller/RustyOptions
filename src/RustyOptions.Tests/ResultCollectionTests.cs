using static RustyOptions.Result;

namespace RustyOptions.Tests;

public class ResultCollectionTests
{
    [Fact]
    public void CanGetValues()
    {
        var results = Enumerable.Range(1, 10)
            .Select(x => (x & 1) == 0 ? Ok(x) : Err<int>("odd"));

        var values = results.Values().ToArray();

        Assert.Equal([2, 4, 6, 8, 10], values);
    }

    [Fact]
    public void CanGetErrors()
    {
        var results = Enumerable.Range(1, 10)
            .Select(x => (x & 1) == 0 ? Ok(x) : Err<int>("odd"));

        var errors = results.Errors().ToArray();

        Assert.Equal(Enumerable.Repeat("odd", 5), errors);
    }
}

