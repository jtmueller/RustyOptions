using System.Runtime.CompilerServices;
using static System.ArgumentNullException;

namespace RustyOptions;

/// <summary>
/// Extension methods for working with collections involving <see cref="Result{T, TErr}"/>.
/// </summary>
public static class ResultCollectionExtensions
{
    /// <summary>
    /// Flattens a sequence of <see cref="Result{T, TErr}"/> into a sequence containing all inner values.
    /// Error results are discarded.
    /// </summary>
    /// <param name="self">The sequence of results.</param>
    /// <returns>A flattened sequence of values.</returns>
    public static IEnumerable<T> Values<T, TErr>(this IEnumerable<Result<T, TErr>> self)
        where T : notnull
        where TErr : notnull
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
    /// Flattens an asynchronous sequence of <see cref="Result{T, TErr}"/> into a sequence containing all inner values.
    /// Error results are discarded.
    /// </summary>
    /// <param name="self">The asynchronsous sequence of results.</param>
    /// <param name="ct">A CancellationToken that will interrupt async iteration.</param>
    /// <returns>A flattened sequence of values.</returns>
    public static async IAsyncEnumerable<T> ValuesAsync<T, TErr>(this IAsyncEnumerable<Result<T, TErr>> self, [EnumeratorCancellation] CancellationToken ct = default)
        where T : notnull
        where TErr : notnull
    {
        ThrowIfNull(self);

        await foreach (var result in self.WithCancellation(ct))
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
    public static IEnumerable<TErr> Errors<T, TErr>(this IEnumerable<Result<T, TErr>> self)
        where T : notnull
        where TErr : notnull
    {
        ThrowIfNull(self);

        foreach (var result in self)
        {
            if (result.IsErr(out var err))
            {
                yield return err;
            }
        }
    }

    /// <summary>
    /// Flattens an asynchronous sequence of <see cref="Result{T, TErr}"/> into a sequence containing all error values.
    /// Ok results are discarded.
    /// </summary>
    /// <param name="self">The asynchronsou sequence of results.</param>
    /// <param name="ct">A CancellationToken that will interrupt async iteration.</param>
    /// <returns>A flattened sequence of values.</returns>
    public static async IAsyncEnumerable<TErr> ErrorsAsync<T, TErr>(this IAsyncEnumerable<Result<T, TErr>> self, [EnumeratorCancellation] CancellationToken ct = default)
        where T : notnull
        where TErr : notnull
    {
        ThrowIfNull(self);

        await foreach (var result in self.WithCancellation(ct))
        {
            if (result.IsErr(out var err))
            {
                yield return err;
            }
        }
    }
}
