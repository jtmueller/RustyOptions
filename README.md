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
 - Clearly express your intent in the code you build. If a method might not return a value, or might return an error message instead of a value, you can express this in the return type where it can't be missed.
 - Provide an [expressive API](https://jtmueller.github.io/RustyOptions/api/RustyOptions.html) that allows chaining and combination of optional or possibly-failed results.

## Usage

### Creating an Option

There are many ways to create an Option instance - use whichever appeals to you!

```csharp
Option<int> ex1 = Option.Some(42);
Option<int> ex2 = 42.Some();
Option<int> ex3 = Option<int>.None;
Option<int> ex4 = Option.None<int>();

int? maybeNull = GetPossiblyNullInteger();
Option<int> ex5 = Option.Create(maybeNull); // null turns into Option.None

// Or you can use 'using static' for more concise/Rust-like syntax:
using static RustyOptions.Option;

var ex6 = Some(42);
var ex7 = None<int>();
```

### Getting values from an Option

```csharp
if (myOption.IsSome(out var value))
{
    // do something with the value
}

// Get the contained value if Some, or the default value for the contained type if None
var innerValue = myOption.UnwrapOr(default);

if (myOption.IsNone)
{
   // Do something because the option is None,
   // although in many cases the Unwrap/Map/And/Or
   // methods are more useful than checking IsNone directly.
}
```

### Creating a Result

Result can also be created in a variety of ways.

```csharp
// assumes an Err type of string
Result<int, string> ex1 = Result.Ok(42);
// assumes an Err type of Exception
Result<int, Exception> ex2 = Result.OkExn(42);
// fully specify Ok and Err types
Result<int, MyCustomException> ex3 = Result.Ok<int, MyCustomException>(42);

Result<int, string> ex4 = Result.Err<int>("Oops.");
Result<int, Exception> ex5 = Result.Err<int>(new Exception("Oops."));
Result<int, MyCustomException> ex6 = Result.Err<int, MyCustomException(someException);

// Or you can use 'using static' for more concise/Rust-like syntax:
using static RustyOptions.Result;

var ex7 = Ok(42);
var ex8 = Err<int>("oops");
```

### Getting values from a Result

```csharp
if (myResult.IsOk(out var value))
{
    // do something with the value
}

if (myResult.IsErr(out var err))
{
    // do something with the error
}
```

### Safely Chain Together Fallible Methods

RustyOptions has an [extensive API](https://jtmueller.github.io/RustyOptions/api/RustyOptions.html) 
that supports safely chaining together multiple methods that return `Option` or `Result` 
and supports easily converting between `Option` and `Result` if you have a mixture of such methods.

```csharp
var output = Option.Parse<int>(userInput)
    .AndThen(ValidateRange)
    .OrElse(() => defaultsByGroup.GetValueOrNone(user.GroupId))
    .MapOr(PillWidget.Render, string.Empty);
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
