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
        var source = Some(42);
        var taskSrc = Task.FromResult(source);
        var valTaskSrc = ValueTask.FromResult(source);

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
    }
}
