using System.Runtime.CompilerServices;
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
        int count = 0;
        await foreach (var x in EnumerateFilteredAsync(11, i => (i & 1) == 0).ValuesAsync())
        {
            Assert.True((x & 1) == 0);
            count++;
        }

        Assert.Equal(6, count);
    }

    [Fact]
    public async Task CanGetValuesWithCancelAsync()
    {
        using var cts = new CancellationTokenSource();

        int count = 0;
        await foreach (var x in EnumerateFilteredAsync(11, i => (i & 1) == 0, cts.Token).ValuesAsync(cts.Token))
        {
            Assert.True((x & 1) == 0);
            count++;

            if (count > 2)
            {
                cts.Cancel();
            }
        }

        Assert.Equal(3, count);
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
        int count = 0;
        await foreach (var err in EnumerateFilteredAsync(11, i => (i & 1) == 0).ErrorsAsync())
        {
            Assert.Equal("predicate failed", err);
            count++;
        }

        Assert.Equal(5, count);
    }

    [Fact]
    public async Task CanGetErrorsWithCancelAsync()
    {
        using var cts = new CancellationTokenSource();

        int count = 0;
        await foreach (var err in EnumerateFilteredAsync(11, i => (i & 1) == 0, cts.Token).ErrorsAsync(cts.Token))
        {
            Assert.Equal("predicate failed", err);
            count++;

            if (count > 2)
            {
                cts.Cancel();
            }
        }

        Assert.Equal(3, count);
    }

    /// <summary>
    /// Generates an IAsyncEnumerable that counts up to, but not including, the given
    /// <paramref name="exclusiveMax"/> - returning Some for numbers for which the predicate returns true
    /// and None otherwise.
    /// </summary>
    private static async IAsyncEnumerable<Result<int, string>> EnumerateFilteredAsync(int exclusiveMax, Func<int, bool> predicate, [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 0; !ct.IsCancellationRequested && i < exclusiveMax; i++)
        {
            await Task.Yield();
            yield return predicate(i) ? Ok(i) : Err<int>("predicate failed");
        }
    }
}

