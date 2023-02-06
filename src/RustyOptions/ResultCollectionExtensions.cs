using static System.ArgumentNullException;

namespace RustyOptions;

/// <summary>
/// Extension methods for working with collections involving <see cref="Result{T, TErr}"/>.
/// </summary>
public static class ResultCollectionExtensions
{
    // NOTE: Due to a bug in coverlet.collector, certain lines in methods involving IAsyncEnumerable
    // will show as partially-covered in code-coverage tools, even when they are fully-covered.
    // https://github.com/coverlet-coverage/coverlet/issues/1104#issuecomment-1005332269

    /// <summary>
    /// Flattens a sequence of <see cref="Result{T, TErr}"/> into a sequence containing all inner values.
    /// Error results are discarded.
    /// </summary>
    /// <param name="self">The sequence of results.</param>
    /// <returns>A flattened sequence of values.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="self"/> is null.</exception>
    public static IEnumerable<T> Values<T, TErr>(this IEnumerable<Result<T, TErr>> self)
        where T : notnull
    {
        ThrowIfNull(self);

        foreach (var result in self)
        {
            if (result.IsOk(out var value))
            {
                yield return value;
            }
        }
    }

    /// <summary>
    /// Flattens a sequence of <see cref="Result{T, TErr}"/> into a sequence containing all error values.
    /// Ok results are discarded.
    /// </summary>
    /// <param name="self">The sequence of results.</param>
    /// <returns>A flattened sequence of error values.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="self"/> is null.</exception>
    public static IEnumerable<TErr> Errors<T, TErr>(this IEnumerable<Result<T, TErr>> self)
        where T : notnull
    {
        ThrowIfNull(self);

        foreach (var result in self)
        {
            if (result.IsErr(out var err) && err is not null)
            {
                yield return err;
            }
        }
    }
}
