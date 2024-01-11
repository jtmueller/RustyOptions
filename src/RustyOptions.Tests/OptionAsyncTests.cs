using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using RustyOptions.Async;
using static RustyOptions.Option;

namespace RustyOptions.Tests;

public sealed class OptionAsyncTests
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
    public async Task CanGetFirstOrNoneAsync()
    {
        Assert.Equal(None<int>(), await EnumerateAsync(0).FirstOrNoneAsync());
        Assert.Equal(Some(0), await EnumerateAsync(11).FirstOrNoneAsync());
        Assert.Equal(Some(2), await EnumerateAsync(11).FirstOrNoneAsync(x => x > 0 && (x & 1) == 0));
        Assert.Equal(None<int>(), await EnumerateAsync(11).FirstOrNoneAsync(x => x > 500));
    }

    [Fact]
    public async Task CanGetFirstOrNoneWithCancelAsync()
    {
        using var cts = new CancellationTokenSource();

        Assert.Equal(None<int>(), await EnumerateAsync(0, cts.Token).FirstOrNoneAsync(cts.Token));
        Assert.Equal(Some(0), await EnumerateAsync(11, cts.Token).FirstOrNoneAsync(cts.Token));
        Assert.Equal(Some(2), await EnumerateAsync(11, cts.Token).FirstOrNoneAsync(x => x > 0 && (x & 1) == 0, cts.Token));
        Assert.Equal(None<int>(), await EnumerateAsync(11, cts.Token).FirstOrNoneAsync(x => x > 500, cts.Token));

        cts.Cancel();

        Assert.Equal(None<int>(), await EnumerateAsync(0, cts.Token).FirstOrNoneAsync(cts.Token));
        Assert.Equal(None<int>(), await EnumerateAsync(11, cts.Token).FirstOrNoneAsync(cts.Token));
        Assert.Equal(None<int>(), await EnumerateAsync(11, cts.Token).FirstOrNoneAsync(x => x > 0 && (x & 1) == 0, cts.Token));
        Assert.Equal(None<int>(), await EnumerateAsync(11, cts.Token).FirstOrNoneAsync(x => x > 500, cts.Token));
    }

    [Theory]
    [MemberData(nameof(GetMapAsyncValues))]
    [SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method", Justification = "Will not block")]
    public async Task CanMapAsync(object source, object mapper, Option<long> expected)
    {
        switch ((source, mapper))
        {
            case (Option<int> src, Func<int, ValueTask<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;
            case (Option<int> src, Func<int, Task<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<int, long> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).MapAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<int, long> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).MapAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<int, ValueTask<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).MapAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<int, ValueTask<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).MapAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<int, Task<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).MapAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<int, Task<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).MapAsync(mpr));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static TheoryData<object, object, Option<long>> GetMapAsyncValues()
    {
        var expected = Some(84L);
        var expectedNone = None<long>();
        var source = Some(42);
        var sourceNone = None<int>();
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
            case (Option<int> src, Func<int, ValueTask<long>> mpr, Func<ValueTask<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;
            case (Option<int> src, Func<int, Task<long>> mpr, Func<Task<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;
            case (ValueTask<Option<int>> src, Func<int, long> mpr, Func<long> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                Assert.Equal(expected, await GetValueTask(src.Result).MapOrElseAsync(mpr, dff));
                break;
            case (Task<Option<int>> src, Func<int, long> mpr, Func<long> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                Assert.Equal(expected, await GetTask(src.Result).MapOrElseAsync(mpr, dff));
                break;
            case (ValueTask<Option<int>> src, Func<int, ValueTask<long>> mpr, Func<ValueTask<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                Assert.Equal(expected, await GetValueTask(src.Result).MapOrElseAsync(mpr, dff));
                break;
            case (Task<Option<int>> src, Func<int, ValueTask<long>> mpr, Func<ValueTask<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                Assert.Equal(expected, await GetTask(src.Result).MapOrElseAsync(mpr, dff));
                break;
            case (ValueTask<Option<int>> src, Func<int, Task<long>> mpr, Func<Task<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                Assert.Equal(expected, await GetValueTask(src.Result).MapOrElseAsync(mpr, dff));
                break;
            case (Task<Option<int>> src, Func<int, Task<long>> mpr, Func<Task<long>> dff):
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
        var defaultFactory = () => -1L;
        var taskDefault = () => Task.FromResult(-1L);
        var valueTaskDefault = () => ValueTask.FromResult(-1L);

        var expected = 84L;
        var expectedNone = -1L;

        var source = Some(42);
        var sourceNone = None<int>();
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
    public async Task CanAndThenAsync(object source, object mapper, Option<long> expected)
    {
        switch ((source, mapper))
        {
            case (Option<int> src, Func<int, ValueTask<Option<long>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;
            case (Option<int> src, Func<int, Task<Option<long>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<int, Option<long>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).AndThenAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<int, Option<long>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).AndThenAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<int, ValueTask<Option<long>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).AndThenAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<int, ValueTask<Option<long>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).AndThenAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<int, Task<Option<long>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).AndThenAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<int, Task<Option<long>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).AndThenAsync(mpr));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static TheoryData<object, object, Option<long>> GetAndThenAsyncValues()
    {
        var expected = Some(84L);
        var expectedNone = None<long>();
        var source = Some(42);
        var sourceNone = None<int>();
        var taskSrc = Task.FromResult(source);
        var valTaskSrc = ValueTask.FromResult(source);
        var taskSrcNone = Task.FromResult(sourceNone);
        var valTaskSrcNone = ValueTask.FromResult(sourceNone);

        var mapper = (int x) => Some((long)(x * 2));
        var taskMapper = (int x) => Task.FromResult(Some((long)(x * 2)));
        var valTaskMapper = (int x) => ValueTask.FromResult(Some((long)(x * 2)));

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
    public async Task CanOrElseAsync(object source, object mapper, Option<int> expected)
    {
        switch ((source, mapper))
        {
            case (Option<int> src, Func<ValueTask<Option<int>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (Option<int> src, Func<Task<Option<int>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<Option<int>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).OrElseAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<Option<int>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).OrElseAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<ValueTask<Option<int>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).OrElseAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<ValueTask<Option<int>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).OrElseAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<Task<Option<int>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                Assert.Equal(expected, await GetValueTask(src.Result).OrElseAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<Task<Option<int>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                Assert.Equal(expected, await GetTask(src.Result).OrElseAsync(mpr));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static TheoryData<object, object, Option<int>> GetOrElseAsyncValues()
    {
        var expected = Some(42);
        var expectedNone = Some(-1);
        var source = Some(42);
        var sourceNone = None<int>();
        var taskSrc = Task.FromResult(source);
        var valTaskSrc = ValueTask.FromResult(source);
        var taskSrcNone = Task.FromResult(sourceNone);
        var valTaskSrcNone = ValueTask.FromResult(sourceNone);

        var mapper = () => Some(-1);
        var taskMapper = () => Task.FromResult(Some(-1));
        var valTaskMapper = () => ValueTask.FromResult(Some(-1));

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
    /// <paramref name="exclusiveMax"/>
    /// </summary>
    private static async IAsyncEnumerable<int> EnumerateAsync(int exclusiveMax, [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 0; !ct.IsCancellationRequested && i < exclusiveMax; i++)
        {
            await Task.Yield();
            yield return i;
        }
    }

    /// <summary>
    /// Generates an IAsyncEnumerable that counts up to, but not including, the given
    /// <paramref name="exclusiveMax"/> - returning Some for numbers for which the predicate returns true
    /// and None otherwise.
    /// </summary>
    private static async IAsyncEnumerable<Option<int>> EnumerateFilteredAsync(int exclusiveMax, Func<int, bool> predicate, [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 0; !ct.IsCancellationRequested && i < exclusiveMax; i++)
        {
            await Task.Yield();
            yield return predicate(i) ? Some(i) : None<int>();
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
