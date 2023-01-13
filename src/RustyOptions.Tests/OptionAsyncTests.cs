using System.Runtime.CompilerServices;
using static RustyOptions.Option;

namespace RustyOptions.Tests;

public sealed class OptionAsyncTests
{
    [Theory]
    [MemberData(nameof(GetMapAsyncValues))]
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
                break;
            case (Task<Option<int>> src, Func<int, long> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<int, ValueTask<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<int, ValueTask<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<int, Task<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<int, Task<long>> mpr):
                Assert.Equal(expected, await src.MapAsync(mpr));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static IEnumerable<object[]> GetMapAsyncValues()
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
    [MemberData(nameof(GetMapOrAsyncValues))]
    public async Task CanMapOrAsync(object source, object mapper, long defaultValue, long expected)
    {
        switch ((source, mapper))
        {
            case (Option<int> src, Func<int, ValueTask<long>> mpr):
                Assert.Equal(expected, await src.MapOrAsync(mpr, defaultValue));
                break;
            case (Option<int> src, Func<int, Task<long>> mpr):
                Assert.Equal(expected, await src.MapOrAsync(mpr, defaultValue));
                break;
            case (ValueTask<Option<int>> src, Func<int, long> mpr):
                Assert.Equal(expected, await src.MapOrAsync(mpr, defaultValue));
                break;
            case (Task<Option<int>> src, Func<int, long> mpr):
                Assert.Equal(expected, await src.MapOrAsync(mpr, defaultValue));
                break;
            case (ValueTask<Option<int>> src, Func<int, ValueTask<long>> mpr):
                Assert.Equal(expected, await src.MapOrAsync(mpr, defaultValue));
                break;
            case (Task<Option<int>> src, Func<int, ValueTask<long>> mpr):
                Assert.Equal(expected, await src.MapOrAsync(mpr, defaultValue));
                break;
            case (ValueTask<Option<int>> src, Func<int, Task<long>> mpr):
                Assert.Equal(expected, await src.MapOrAsync(mpr, defaultValue));
                break;
            case (Task<Option<int>> src, Func<int, Task<long>> mpr):
                Assert.Equal(expected, await src.MapOrAsync(mpr, defaultValue));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static IEnumerable<object[]> GetMapOrAsyncValues()
    {
        var defaultValue = -1L;
        var expected = 84L;
        var expectedNone = defaultValue;
        var source = Some(42);
        var sourceNone = None<int>();
        var taskSrc = Task.FromResult(source);
        var valTaskSrc = ValueTask.FromResult(source);
        var taskSrcNone = Task.FromResult(sourceNone);
        var valTaskSrcNone = ValueTask.FromResult(sourceNone);

        var mapper = (int x) => (long)(x * 2);
        var taskMapper = (int x) => Task.FromResult((long)(x * 2));
        var valTaskMapper = (int x) => ValueTask.FromResult((long)(x * 2));

        yield return new object[] { source, taskMapper, defaultValue, expected };
        yield return new object[] { source, valTaskMapper, defaultValue, expected };
        yield return new object[] { taskSrc, mapper, defaultValue, expected };
        yield return new object[] { valTaskSrc, mapper, defaultValue, expected };
        yield return new object[] { taskSrc, taskMapper, defaultValue, expected };
        yield return new object[] { valTaskSrc, taskMapper, defaultValue, expected };
        yield return new object[] { taskSrc, valTaskMapper, defaultValue, expected };
        yield return new object[] { valTaskSrc, valTaskMapper, defaultValue, expected };

        yield return new object[] { sourceNone, taskMapper, defaultValue, expectedNone };
        yield return new object[] { sourceNone, valTaskMapper, defaultValue, expectedNone };
        yield return new object[] { taskSrcNone, mapper, defaultValue, expectedNone };
        yield return new object[] { valTaskSrcNone, mapper, defaultValue, expectedNone };
        yield return new object[] { taskSrcNone, taskMapper, defaultValue, expectedNone };
        yield return new object[] { valTaskSrcNone, taskMapper, defaultValue, expectedNone };
        yield return new object[] { taskSrcNone, valTaskMapper, defaultValue, expectedNone };
        yield return new object[] { valTaskSrcNone, valTaskMapper, defaultValue, expectedNone };
    }

    [Theory]
    [MemberData(nameof(GetMapOrElseAsyncValues))]
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
                break;
            case (Task<Option<int>> src, Func<int, long> mpr, Func<long> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;
            case (ValueTask<Option<int>> src, Func<int, ValueTask<long>> mpr, Func<ValueTask<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;
            case (Task<Option<int>> src, Func<int, ValueTask<long>> mpr, Func<ValueTask<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;
            case (ValueTask<Option<int>> src, Func<int, Task<long>> mpr, Func<Task<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;
            case (Task<Option<int>> src, Func<int, Task<long>> mpr, Func<Task<long>> dff):
                Assert.Equal(expected, await src.MapOrElseAsync(mpr, dff));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static IEnumerable<object[]> GetMapOrElseAsyncValues()
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
                break;
            case (Task<Option<int>> src, Func<int, Option<long>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<int, ValueTask<Option<long>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<int, ValueTask<Option<long>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<int, Task<Option<long>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<int, Task<Option<long>>> mpr):
                Assert.Equal(expected, await src.AndThenAsync(mpr));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static IEnumerable<object[]> GetAndThenAsyncValues()
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
                break;
            case (Task<Option<int>> src, Func<Option<int>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<ValueTask<Option<int>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<ValueTask<Option<int>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (ValueTask<Option<int>> src, Func<Task<Option<int>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;
            case (Task<Option<int>> src, Func<Task<Option<int>>> mpr):
                Assert.Equal(expected, await src.OrElseAsync(mpr));
                break;

            default:
                Assert.Fail($"Unexpected source/mapper: {source.GetType().Name}/{mapper.GetType().Name}");
                break;
        }
    }

    public static IEnumerable<object[]> GetOrElseAsyncValues()
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
