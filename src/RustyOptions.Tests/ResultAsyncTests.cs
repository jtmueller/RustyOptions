using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using RustyOptions.Async;
using static RustyOptions.Result;

namespace RustyOptions.Tests;

public class ResultAsyncTests
{
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

    [Theory]
    [MemberData(nameof(GetMapAsyncValues))]
    [SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method", Justification = "Will not block")]
    public async Task CanMapAsync(object source, object mapper, Result<long, string> expected)
    {
        switch ((source, mapper))
        {
            case (Result<int, string> src, Func<int, ValueTask<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;
            case (Result<int, string> src, Func<int, Task<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, long> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).MapAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<int, long> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).MapAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, ValueTask<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).MapAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<int, ValueTask<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).MapAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, Task<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).MapAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<int, Task<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).MapAsync(mpr));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static TheoryData<object, object, Result<long, string>> GetMapAsyncValues()
    {
        var expected = Ok(84L);
        var expectedNone = Err<long>("no value");
        var source = Ok(42);
        var sourceNone = Err<int>("no value");
        var taskSrc = Task.FromResult(source);
        var valTaskSrc = ValueTask.FromResult(source);
        var taskSrcNone = Task.FromResult(sourceNone);
        var valTaskSrcNone = ValueTask.FromResult(sourceNone);

        var mapper = (int x) => (long)(x * 2);
        var taskMapper = (int x) => Task.FromResult((long)(x * 2));
        var valTaskMapper = (int x) => ValueTask.FromResult((long)(x * 2));

        return new()
        {
            { source, taskMapper, expected },
            { source, valTaskMapper, expected },
            { taskSrc, mapper, expected },
            { valTaskSrc, mapper, expected },
            { taskSrc, taskMapper, expected },
            { valTaskSrc, taskMapper, expected },
            { taskSrc, valTaskMapper, expected },
            { valTaskSrc, valTaskMapper, expected },

            { sourceNone, taskMapper, expectedNone },
            { sourceNone, valTaskMapper, expectedNone },
            { taskSrcNone, mapper, expectedNone },
            { valTaskSrcNone, mapper, expectedNone },
            { taskSrcNone, taskMapper, expectedNone },
            { valTaskSrcNone, taskMapper, expectedNone },
            { taskSrcNone, valTaskMapper, expectedNone },
            { valTaskSrcNone, valTaskMapper, expectedNone },
        };
    }

    [Theory]
    [MemberData(nameof(GetMapOrElseAsyncValues))]
    [SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method", Justification = "Will not block")]
    public async Task CanMapOrElseAsync(object source, object mapper, object defaultFactory, long expected)
    {
        switch ((source, mapper, defaultFactory))
        {
            case (Result<int, string> src, Func<int, ValueTask<long>> mpr, Func<string, ValueTask<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;
            case (Result<int, string> src, Func<int, Task<long>> mpr, Func<string, Task<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, long> mpr, Func<string, long> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                Assert.Equal(expected, await GetValueTask(src.Result).MapOrElseAsync(mpr, dff));
                break;
            case (Task<Result<int, string>> src, Func<int, long> mpr, Func<string, long> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                Assert.Equal(expected, await GetTask(src.Result).MapOrElseAsync(mpr, dff));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, ValueTask<long>> mpr, Func<string, ValueTask<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                Assert.Equal(expected, await GetValueTask(src.Result).MapOrElseAsync(mpr, dff));
                break;
            case (Task<Result<int, string>> src, Func<int, ValueTask<long>> mpr, Func<string, ValueTask<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                Assert.Equal(expected, await GetTask(src.Result).MapOrElseAsync(mpr, dff));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, Task<long>> mpr, Func<string, Task<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                Assert.Equal(expected, await GetValueTask(src.Result).MapOrElseAsync(mpr, dff));
                break;
            case (Task<Result<int, string>> src, Func<int, Task<long>> mpr, Func<string, Task<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                Assert.Equal(expected, await GetTask(src.Result).MapOrElseAsync(mpr, dff));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static TheoryData<object, object, object, long> GetMapOrElseAsyncValues()
    {
        var defaultFactory = (string _) => -1L;
        var taskDefault = (string _) => Task.FromResult(-1L);
        var valueTaskDefault = (string _) => ValueTask.FromResult(-1L);

        var expected = 84L;
        var expectedNone = -1L;

        var source = Ok(42);
        var sourceNone = Err<int>("oops");
        var taskSrc = Task.FromResult(source);
        var valTaskSrc = ValueTask.FromResult(source);
        var taskSrcNone = Task.FromResult(sourceNone);
        var valTaskSrcNone = ValueTask.FromResult(sourceNone);

        var mapper = (int x) => (long)(x * 2);
        var taskMapper = (int x) => Task.FromResult((long)(x * 2));
        var valTaskMapper = (int x) => ValueTask.FromResult((long)(x * 2));

        return new()
        {
            { source, taskMapper, taskDefault, expected },
            { source, valTaskMapper, valueTaskDefault, expected },
            { taskSrc, mapper, defaultFactory, expected },
            { valTaskSrc, mapper, defaultFactory, expected },
            { taskSrc, taskMapper, taskDefault, expected },
            { valTaskSrc, taskMapper, taskDefault, expected },
            { taskSrc, valTaskMapper, valueTaskDefault, expected },
            { valTaskSrc, valTaskMapper, valueTaskDefault, expected },

            { sourceNone, taskMapper, taskDefault, expectedNone },
            { sourceNone, valTaskMapper, valueTaskDefault, expectedNone },
            { taskSrcNone, mapper, defaultFactory, expectedNone },
            { valTaskSrcNone, mapper, defaultFactory, expectedNone },
            { taskSrcNone, taskMapper, taskDefault, expectedNone },
            { valTaskSrcNone, taskMapper, taskDefault, expectedNone },
            { taskSrcNone, valTaskMapper, valueTaskDefault, expectedNone },
            { valTaskSrcNone, valTaskMapper, valueTaskDefault, expectedNone },
        };
    }

    [Theory]
    [MemberData(nameof(GetAndThenAsyncValues))]
    [SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method", Justification = "Will not block")]
    public async Task CanAndThenAsync(object source, object mapper, Result<long, string> expected)
    {
        switch ((source, mapper))
        {
            case (Result<int, string> src, Func<int, ValueTask<Result<long, string>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;
            case (Result<int, string> src, Func<int, Task<Result<long, string>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, Result<long, string>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).AndThenAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<int, Result<long, string>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).AndThenAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, ValueTask<Result<long, string>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).AndThenAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<int, ValueTask<Result<long, string>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).AndThenAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, Task<Result<long, string>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).AndThenAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<int, Task<Result<long, string>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).AndThenAsync(mpr));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static TheoryData<object, object, Result<long, string>> GetAndThenAsyncValues()
    {
        var expected = Ok(84L);
        var expectedNone = Err<long>("oops");
        var source = Ok(42);
        var sourceNone = Err<int>("oops");
        var taskSrc = Task.FromResult(source);
        var valTaskSrc = ValueTask.FromResult(source);
        var taskSrcNone = Task.FromResult(sourceNone);
        var valTaskSrcNone = ValueTask.FromResult(sourceNone);

        var mapper = (int x) => Ok((long)(x * 2));
        var taskMapper = (int x) => Task.FromResult(Ok((long)(x * 2)));
        var valTaskMapper = (int x) => ValueTask.FromResult(Ok((long)(x * 2)));

        return new()
        {
            { source, taskMapper, expected },
            { source, valTaskMapper, expected },
            { taskSrc, mapper, expected },
            { valTaskSrc, mapper, expected },
            { taskSrc, taskMapper, expected },
            { valTaskSrc, taskMapper, expected },
            { taskSrc, valTaskMapper, expected },
            { valTaskSrc, valTaskMapper, expected },

            { sourceNone, taskMapper, expectedNone },
            { sourceNone, valTaskMapper, expectedNone },
            { taskSrcNone, mapper, expectedNone },
            { valTaskSrcNone, mapper, expectedNone },
            { taskSrcNone, taskMapper, expectedNone },
            { valTaskSrcNone, taskMapper, expectedNone },
            { taskSrcNone, valTaskMapper, expectedNone },
            { valTaskSrcNone, valTaskMapper, expectedNone },
        };
    }

    [Theory]
    [MemberData(nameof(GetOrElseAsyncValues))]
    [SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method", Justification = "Will not block")]
    public async Task CanOrElseAsync(object source, object mapper, Result<int, string> expected)
    {
        switch ((source, mapper))
        {
            case (Result<int, string> src, Func<string?, ValueTask<Result<int, string>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (Result<int, string> src, Func<string?, Task<Result<int, string>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<string?, Result<int, string>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).OrElseAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<string?, Result<int, string>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).OrElseAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<string?, ValueTask<Result<int, string>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).OrElseAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<string?, ValueTask<Result<int, string>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).OrElseAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<string?, Task<Result<int, string>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).OrElseAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<string?, Task<Result<int, string>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).OrElseAsync(mpr));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static TheoryData<object, object, Result<int, string>> GetOrElseAsyncValues()
    {
        var expected = Ok(42);
        var expectedNone = Ok(-1);
        var source = Ok(42);
        var sourceNone = Err<int>("oops");
        var taskSrc = Task.FromResult(source);
        var valTaskSrc = ValueTask.FromResult(source);
        var taskSrcNone = Task.FromResult(sourceNone);
        var valTaskSrcNone = ValueTask.FromResult(sourceNone);

        var mapper = (string _) => Ok(-1);
        var taskMapper = (string _) => Task.FromResult(Ok(-1));
        var valTaskMapper = (string _) => ValueTask.FromResult(Ok(-1));

        return new()
        {
            { source, taskMapper, expected },
            { source, valTaskMapper, expected },
            { taskSrc, mapper, expected },
            { valTaskSrc, mapper, expected },
            { taskSrc, taskMapper, expected },
            { valTaskSrc, taskMapper, expected },
            { taskSrc, valTaskMapper, expected },
            { valTaskSrc, valTaskMapper, expected },

            { sourceNone, taskMapper, expectedNone },
            { sourceNone, valTaskMapper, expectedNone },
            { taskSrcNone, mapper, expectedNone },
            { valTaskSrcNone, mapper, expectedNone },
            { taskSrcNone, taskMapper, expectedNone },
            { valTaskSrcNone, taskMapper, expectedNone },
            { taskSrcNone, valTaskMapper, expectedNone },
            { valTaskSrcNone, valTaskMapper, expectedNone },
        };
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

    private static async Task<T> GetTask<T>(T value)
    {
        await Task.Delay(1);
        return value;
    }

    private static async ValueTask<T> GetValueTask<T>(T value)
    {
        await Task.Delay(1);
        return value;
    }
}

