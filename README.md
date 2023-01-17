# RustyOptions
Option and Result types for C#, inspired by Rust

[![CI](https://github.com/jtmueller/RustyOptions/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/jtmueller/RustyOptions/actions/workflows/build-and-test.yml) 
[![CodeQL](https://github.com/jtmueller/RustyOptions/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/jtmueller/RustyOptions/actions/workflows/codeql-analysis.yml) 
[![codecov](https://codecov.io/gh/jtmueller/RustyOptions/branch/main/graph/badge.svg?token=M81EJH4ZEI)](https://codecov.io/gh/jtmueller/RustyOptions)


## Avoid Null-Reference Errors

The C# [nullable reference types](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references) feature is useful,
but it's entirely optional, and the warnings it produces are easily ignored.

RustyOptions uses the type system to:
 - Make it impossible to access a possibly-missing value without first checking if the value is present.
 - Clearly express your intent in the API you build. If a method might not return a value, or might return an error message instead of a value, you can express this in the return type where it can't be missed.


## Safely Chain Together Fallible Methods

```csharp
var output = Option.Parse<int>(userInput)
    .AndThen(ValidateRange)
    .OrElse(() => defaultsByGroup.GetValueOrNone(user.GroupId))
    .MapOr(UIRangeWidget.Render, string.Empty);
```

The example above does the following:
 1. Attempts to parse the user input into an integer.
 2. If the parsing succeeds, passes the resulting number to the `ValidateRange` method, which returns `Some(parsedInput)`
    if the parsed input is within the valid range, or `None` if it falls outside the valid range.
 3. If either steps 1 or 2 fail, we attempt to do a dictionary lookup to get a default value using the current user's group ID.
 4. If at the end we have a value, we render it to a string. Otherwise, we set `output` to an empty string.


## Uses Modern .NET Features

For performance and convenience:
 - Supports parsing any type that implements `IParsable<T>` or `ISpanParsable<T>` (.NET 7 and above only)
 - The `NumericOption<T>` type supports generic math for any contained type that implements `INumber<T>` (.NET 7 and above only)
 - Supports `async` and `IAsyncEnumerable<T>`.
 - Supports nullable type annotations.
 - Supports serialization and deserialization with `System.Text.Json`.
 - `IEquatable<T>` and `IComparable<T>` allow `Option` and `Result` types to be easily compared and sorted.
 - `IFormattable` and `ISpanFormattable` allow `Option` and `Result` to efficiently format their content.
 - `Option` and `Result` can be efficiently converted to `ReadOnlySpan<T>` or `IEnumerable<T>` for easier interop with existing code.
 - Convenient extension methods for working with dictionaries (`GetValueOrNone`), collections (`FirstOrNone`), enums (`Option.ParseEnum`) and more.