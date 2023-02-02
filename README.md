# RustyOptions
Option and Result types for C#, inspired by Rust

[![CI](https://github.com/jtmueller/RustyOptions/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/jtmueller/RustyOptions/actions/workflows/build-and-test.yml) 
[![CodeQL](https://github.com/jtmueller/RustyOptions/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/jtmueller/RustyOptions/actions/workflows/codeql-analysis.yml) 
[![codecov](https://codecov.io/gh/jtmueller/RustyOptions/branch/main/graph/badge.svg?token=M81EJH4ZEI)](https://codecov.io/gh/jtmueller/RustyOptions)
[![NuGet](https://buildstats.info/nuget/RustyOptions)](https://www.nuget.org/packages/RustyOptions/)

```
dotnet add package RustyOptions
```

## Avoid Null-Reference Errors

The C# [nullable reference types](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references) feature is useful,
but it's entirely optional, and the warnings it produces are easily ignored.

RustyOptions uses the type system to:
 - Make it impossible to access a possibly-missing value without first checking if the value is present.
 - Clearly express your intent in the code you build. If a method might not return a value, or might return an error message instead of a value, you can express this in the return type where it can't be missed.
 - Provide an [expressive API](https://jtmueller.github.io/RustyOptions/api/RustyOptions.html) that allows chaining and combination of optional or possibly-failed results.

## Usage

### Creating an Option

There are many ways to create an `Option` instance - use whichever appeals to you!

```csharp
Option<int> ex1 = Option.Some(42);
Option<int> ex2 = 42.Some();
Option<int> ex3 = new Option<int>(42);
Option<int> ex4 = Option<int>.None;
Option<int> ex5 = Option.None<int>();
Option<int> ex6 = default; // equivalent to Option.None<int>()
Option<int> ex7 = 0.None(); // value is used only to determine type

int? maybeNull = GetPossiblyNullInteger();
Option<int> ex8 = Option.Create(maybeNull); // null turns into Option.None

// Or you can use 'using static' for more concise/F#/Rust-like syntax:
using static RustyOptions.Option;

var ex9 = Some(42);
var ex10 = None<int>();
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

`Result` can also be created in a variety of ways.

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

// Or you can use 'using static' for more concise/F#/Rust-like syntax:
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

### Parsing (.NET 7+ only)

Any current or future type that supports `IParsable<T>` or `ISpanParsable<T>` can be conditionally parsed into an `Option<T>` or `NumericOption<T>`.

```csharp
Option<int> integer = Option.Parse<int>("12345");
Option<DateTime> date = Option.Parse<DateTime>("2023-06-17");
Option<TimeSpan> timespan = Option.Parse<TimeSpan>("05:11:04");
Option<double> fraction = Option.Parse<double>("3.14");
Option<Guid> guid = Option.Parse<Guid>("ac439fd6-9b64-42f3-bc74-38017c97b965");
Option<int> nothing = Option.Parse<int>("foo");
```

### Generic Math (.NET 7+ only)

Doing math with `NumericOption<int>` is similar to doing math with `Nullable<int>`. Anything combined with `None` comes out as `None`, but if all values involved are `Some` then the math operations are performed as normal on the inner type, and results are wrapped in `Some`.

```csharp
using static RustyOptions.NumericOption;

var a = Some(3);
var b = Some(5);

Assert.Equal(Some(15), a * b);

// Implicit conversion allows you to mix raw numbers and options in the same expression:
Assert.Equal(Some(25), b * 5);

// You can even use Option<int> as the index value in a for loop!
for (var i = Some(0); i < 5; i++)
{
    // If you set i to None inside the loop,
    // the loop will exit when the current iteration completes,
    // as None is not less than 5.
    i = None<int>();
}
```

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
 - Supports explicit conversion to and from the F# Option, ValueOption, and Result types for easy interop.

## FAQ

  - This library only supports .NET 6 and above. What about .NET Framework? .NET 5? .NET Core 3.1?
    - You may want to consider the [Optional](https://github.com/nlkl/Optional) library for legacy framework support.
    - .NET Core 3.1 and .NET 5 are not supported because as of this writing they are no longer supported by Microsoft.
      However, if these runtimes are important to you, we welcome pull requests.
  - Why create this library if [Optional](https://github.com/nlkl/Optional) already exists?
    - I prefer the Rust Option/Result API methods and wanted to replicate those in C#.
    - I wanted to take advantage of modern .NET features like `ISpanParsable<T>` and `INumber<T>`.
    - I think having distinct `Option` and `Result` types is more clear than having two different kinds of Option.
    - As of this writing, Optional hasn't been updated in five years.
  - What about F# `Option` and `Result`?
    - The `RustyOptions.FSharp` nuget package will allow you to convert between F# and RustyOptions types.
