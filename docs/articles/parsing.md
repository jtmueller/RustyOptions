# Parsing

`Option.ParseEnum<T>()` is a wrapper for `Enum.TryParse` that returns an `Option<T>` based on whether the parsing was successful.
This method is available in all versions of RustyOptions.

## Generic Parsing

_NOTE: Generic Parsing is supported by .NET 7 and above only. This feature is not available in .NET 6._

.NET 7 introduced the [`IParsable<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.iparsable-1?view=net-7.0) and
[`ISpanParsable<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.ispanparsable-1?view=net-7.0) interfaces, and implemented
these interfaces for most primitive types in the framework.

RustyOptions can parse any type into an `Option<T>` that implements either of these interfaces, through the `Option.Parse<T>()` method.

```csharp

// get all the integer values from an enumerable of strings, 
// discarding any strings that could not be parsed into an integer

IEnumerable<string> input = GetInput();

IEnumerable<int> integers = input.Select(Option.Parse<int>).Values();

```
