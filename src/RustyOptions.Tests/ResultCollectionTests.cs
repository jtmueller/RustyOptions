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

        Assert.Equal(new[] { 2, 4, 6, 8, 10 }, values);
    }

    [Fact]
    public async Task CanGetValuesAsync()
    {
        var results = EnumerateFilteredAsync(11, i => (i & 1) == 0);

        await foreach (var x in results.ValuesAsync())
        {
            Assert.True((x & 1) == 0);
        }
    }

    [Fact]
    public void CanGetErrors()
    {
        var results = Enumerable.Range(1, 10)
            .Select(x => (x & 1) == 0 ? Ok(x) : Err<int>("odd"));

        var errors = results.Errors().ToArray();

        Assert.Equal(Enumerable.Repeat("odd", 5), errors);
    }

    [Fact]
    public async Task CanGetErrorsAsync()
    {
        var results = EnumerateFilteredAsync(11, i => (i & 1) == 0);

        int count = 0;
        await foreach (var x in results.ErrorsAsync())
        {
            Assert.Equal("odd", x);
            count++;
        }

        Assert.Equal(5, count);
    }

    /// <summary>
    /// Generates an IAsyncEnumerable that counts up to, but not including, the given
    /// <paramref name="exclusiveMax"/> - returning Some for numbers for which the predicate returns true
    /// and None otherwise.
    /// </summary>
    private static async IAsyncEnumerable<Result<int, string>> EnumerateFilteredAsync(int exclusiveMax, Func<int, bool> predicate)
    {
        for (int i = 0; i < exclusiveMax; i++)
        {
            await Task.Yield();
            yield return predicate(i) ? Ok(i) : Err<int>("odd");
        }
    }
}

