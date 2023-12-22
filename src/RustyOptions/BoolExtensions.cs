
using System.Runtime.CompilerServices;

namespace RustyOptions;

/// <summary>
/// Provides extension methods for boolean values.
/// </summary>
public static class BoolExtensions
{
    /// <summary>
    /// Executes a function if the boolean value is true and returns an option.
    /// </summary>
    /// <typeparam name="T">The type of the option value.</typeparam>
    /// <param name="self">The boolean value.</param>
    /// <param name="thenFunc">The function to execute if the boolean value is true.</param>
    /// <returns>An option containing the result of the function if the boolean value is true, otherwise the default option value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Then<T>(this bool self, Func<T> thenFunc)
        where T : notnull
    {
        return self ? new(thenFunc()) : default;
    }

    /// <summary>
    /// Creates an <see cref="Option{T}"/> with a value if the boolean condition is true, otherwise returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="self">The boolean condition.</param>
    /// <param name="value">The value to be wrapped in the option.</param>
    /// <returns>An <see cref="Option{T}"/> with the specified value if the condition is true, otherwise the default value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> ThenSome<T>(this bool self, T value)
        where T : notnull
    {
        return self ? new(value) : default;
    }
}