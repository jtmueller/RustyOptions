
namespace RustyOptions.Async;

/// <summary>
/// Provides extension methods for boolean values.
/// </summary>
public static class BoolAsyncExtensions
{
    /// <summary>
    /// Executes an asynchronous function if the boolean value is true and returns an option.
    /// </summary>
    /// <typeparam name="T">The type of the option value.</typeparam>
    /// <param name="self">The boolean value.</param>
    /// <param name="thenFunc">The function to execute if the boolean value is true.</param>
    /// <returns>An option containing the result of the function if the boolean value is true, otherwise the default option value.</returns>
    public static async ValueTask<Option<T>> ThenAsync<T>(this bool self, Func<Task<T>> thenFunc)
        where T : notnull
    {
        return self ? new(await thenFunc().ConfigureAwait(false)) : default;
    }

    /// <summary>
    /// Executes an asynchronous function if the boolean value is true and returns an option.
    /// </summary>
    /// <typeparam name="T">The type of the option value.</typeparam>
    /// <param name="self">The boolean value.</param>
    /// <param name="thenFunc">The function to execute if the boolean value is true.</param>
    /// <returns>An option containing the result of the function if the boolean value is true, otherwise the default option value.</returns>
    public static async ValueTask<Option<T>> ThenAsync<T>(this bool self, Func<ValueTask<T>> thenFunc)
        where T : notnull
    {
        return self ? new(await thenFunc().ConfigureAwait(false)) : default;
    }

    /// <summary>
    /// Executes the specified task and returns an <see cref="Option{T}"/> with the result if the boolean condition is true.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="self">The boolean condition.</param>
    /// <param name="value">The task to execute.</param>
    /// <returns>An <see cref="Option{T}"/> with the result if the boolean condition is true; otherwise, the default value of <see cref="Option{T}"/>.</returns>
    public static async ValueTask<Option<T>> ThenSomeAsync<T>(this bool self, Task<T> value)
        where T : notnull
    {
        return self
            ? value.IsCompleted ? new(value.Result) : new(await value.ConfigureAwait(false))
            : default;
    }

    /// <summary>
    /// Executes the specified task and returns an <see cref="Option{T}"/> with the result if the boolean condition is true.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="self">The boolean condition.</param>
    /// <param name="value">The task to execute.</param>
    /// <returns>An <see cref="Option{T}"/> with the result if the boolean condition is true; otherwise, the default value of <see cref="Option{T}"/>.</returns>
    public static async ValueTask<Option<T>> ThenSomeAsync<T>(this bool self, ValueTask<T> value)
        where T : notnull
    {
        return self
            ? value.IsCompleted ? new(value.Result) : new(await value.ConfigureAwait(false))
            : default;
    }
}