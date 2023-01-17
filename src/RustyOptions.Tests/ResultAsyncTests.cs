using RustyOptions.Async;
using static RustyOptions.Result;

namespace RustyOptions.Tests;

public class ResultAsyncTests
{
    [Theory]
    [MemberData(nameof(GetMapAsyncValues))]
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
                break;
            case (Task<Result<int, string>> src, Func<int, long> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, ValueTask<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<int, ValueTask<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, Task<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<int, Task<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static IEnumerable<object[]> GetMapAsyncValues()
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

        yield return new object[] { source, taskMapper, expected };
        yield return new object[] { source, valTaskMapper, expected };
        yield return new object[] { taskSrc, mapper, expected };
        yield return new object[] { valTaskSrc, mapper, expected };
        yield return new object[] { taskSrc, taskMapper, expected };
        yield return new object[] { valTaskSrc, taskMapper, expected };
        yield return new object[] { taskSrc, valTaskMapper, expected };
        yield return new object[] { valTaskSrc, valTaskMapper, expected };

        yield return new object[] { sourceNone, taskMapper, expectedNone };
        yield return new object[] { sourceNone, valTaskMapper, expectedNone };
        yield return new object[] { taskSrcNone, mapper, expectedNone };
        yield return new object[] { valTaskSrcNone, mapper, expectedNone };
        yield return new object[] { taskSrcNone, taskMapper, expectedNone };
        yield return new object[] { valTaskSrcNone, taskMapper, expectedNone };
        yield return new object[] { taskSrcNone, valTaskMapper, expectedNone };
        yield return new object[] { valTaskSrcNone, valTaskMapper, expectedNone };
    }

    [Theory]
    [MemberData(nameof(GetMapOrElseAsyncValues))]
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
                break;
            case (Task<Result<int, string>> src, Func<int, long> mpr, Func<string, long> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, ValueTask<long>> mpr, Func<string, ValueTask<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;
            case (Task<Result<int, string>> src, Func<int, ValueTask<long>> mpr, Func<string, ValueTask<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, Task<long>> mpr, Func<string, Task<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;
            case (Task<Result<int, string>> src, Func<int, Task<long>> mpr, Func<string, Task<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static IEnumerable<object[]> GetMapOrElseAsyncValues()
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

        yield return new object[] { source, taskMapper, taskDefault, expected };
        yield return new object[] { source, valTaskMapper, valueTaskDefault, expected };
        yield return new object[] { taskSrc, mapper, defaultFactory, expected };
        yield return new object[] { valTaskSrc, mapper, defaultFactory, expected };
        yield return new object[] { taskSrc, taskMapper, taskDefault, expected };
        yield return new object[] { valTaskSrc, taskMapper, taskDefault, expected };
        yield return new object[] { taskSrc, valTaskMapper, valueTaskDefault, expected };
        yield return new object[] { valTaskSrc, valTaskMapper, valueTaskDefault, expected };

        yield return new object[] { sourceNone, taskMapper, taskDefault, expectedNone };
        yield return new object[] { sourceNone, valTaskMapper, valueTaskDefault, expectedNone };
        yield return new object[] { taskSrcNone, mapper, defaultFactory, expectedNone };
        yield return new object[] { valTaskSrcNone, mapper, defaultFactory, expectedNone };
        yield return new object[] { taskSrcNone, taskMapper, taskDefault, expectedNone };
        yield return new object[] { valTaskSrcNone, taskMapper, taskDefault, expectedNone };
        yield return new object[] { taskSrcNone, valTaskMapper, valueTaskDefault, expectedNone };
        yield return new object[] { valTaskSrcNone, valTaskMapper, valueTaskDefault, expectedNone };
    }

    [Theory]
    [MemberData(nameof(GetAndThenAsyncValues))]
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
                break;
            case (Task<Result<int, string>> src, Func<int, Result<long, string>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, ValueTask<Result<long, string>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<int, ValueTask<Result<long, string>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<int, Task<Result<long, string>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<int, Task<Result<long, string>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static IEnumerable<object[]> GetAndThenAsyncValues()
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

        yield return new object[] { source, taskMapper, expected };
        yield return new object[] { source, valTaskMapper, expected };
        yield return new object[] { taskSrc, mapper, expected };
        yield return new object[] { valTaskSrc, mapper, expected };
        yield return new object[] { taskSrc, taskMapper, expected };
        yield return new object[] { valTaskSrc, taskMapper, expected };
        yield return new object[] { taskSrc, valTaskMapper, expected };
        yield return new object[] { valTaskSrc, valTaskMapper, expected };

        yield return new object[] { sourceNone, taskMapper, expectedNone };
        yield return new object[] { sourceNone, valTaskMapper, expectedNone };
        yield return new object[] { taskSrcNone, mapper, expectedNone };
        yield return new object[] { valTaskSrcNone, mapper, expectedNone };
        yield return new object[] { taskSrcNone, taskMapper, expectedNone };
        yield return new object[] { valTaskSrcNone, taskMapper, expectedNone };
        yield return new object[] { taskSrcNone, valTaskMapper, expectedNone };
        yield return new object[] { valTaskSrcNone, valTaskMapper, expectedNone };
    }

    [Theory]
    [MemberData(nameof(GetOrElseAsyncValues))]
    public async Task CanOrElseAsync(object source, object mapper, Result<int, string> expected)
    {
        switch ((source, mapper))
        {
            case (Result<int, string> src, Func<string, ValueTask<Result<int, string>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (Result<int, string> src, Func<string, Task<Result<int, string>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<string, Result<int, string>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<string, Result<int, string>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<string, ValueTask<Result<int, string>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<string, ValueTask<Result<int, string>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (ValueTask<Result<int, string>> src, Func<string, Task<Result<int, string>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (Task<Result<int, string>> src, Func<string, Task<Result<int, string>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static IEnumerable<object[]> GetOrElseAsyncValues()
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

        yield return new object[] { source, taskMapper, expected };
        yield return new object[] { source, valTaskMapper, expected };
        yield return new object[] { taskSrc, mapper, expected };
        yield return new object[] { valTaskSrc, mapper, expected };
        yield return new object[] { taskSrc, taskMapper, expected };
        yield return new object[] { valTaskSrc, taskMapper, expected };
        yield return new object[] { taskSrc, valTaskMapper, expected };
        yield return new object[] { valTaskSrc, valTaskMapper, expected };

        yield return new object[] { sourceNone, taskMapper, expectedNone };
        yield return new object[] { sourceNone, valTaskMapper, expectedNone };
        yield return new object[] { taskSrcNone, mapper, expectedNone };
        yield return new object[] { valTaskSrcNone, mapper, expectedNone };
        yield return new object[] { taskSrcNone, taskMapper, expectedNone };
        yield return new object[] { valTaskSrcNone, taskMapper, expectedNone };
        yield return new object[] { taskSrcNone, valTaskMapper, expectedNone };
        yield return new object[] { valTaskSrcNone, valTaskMapper, expectedNone };
    }
}

