# Options

The `Option` type is used when an actual value might not exist. 
It avoids the [billion-dollar mistake](https://en.wikipedia.org/wiki/Null_pointer#History) of null references
by making it impossible to forget to check whether a possibly-missing value is present.

## Remarks

The following code illustrates a function which generates an option type.

```csharp
using RustyOptions;

Option<int> KeepIfPositive(int x) => x > 0 ? Option.Some(x) : Option<int>.None;
```

You can take advantage of the `using static` feature of C# for more concise syntax that is closer to
that of languages with built-in Option types, such as F# or Rust.

```csharp
using RustyOptions;
using static RustyOptions.Option;

Option<int> KeepIfPositive(int x) => x > 0 ? Some(x) : None<int>();
```

The value `None` is used when an option does not have the actual value.

## Using Options

Options are commonly used when a search does not return a matching result, as shown in the following code.

```csharp
using RustyOptions;
using static RustyOptions.Option;

Option<T> TryFindMatch<T>(IEnumerable<T> list, Func<T, bool> predicate)
{
    foreach (var value in list)
    {
        if (predicate(value))
        {
            return Some(value);
        }
    }

    return None<T>();
}

// result1 is Some(100) and its type is Option<int>
var result1 = TryFindMatch(new[] { 200, 100, 50, 25 }, x => x == 100);

// result2 is None and its type is Option<char>
var result2 = TryFindMatch(new[] { 'a', 'b', 'c', 'd' }, x => x == 'y');
```

In the previous code, the function `TryFindMatch` takes a list of values and a predicate function
that returns a Boolean value. The function iterates the list, and if an element is found that satisfies
the predicate, the iteration ends and the matching value is returned wrapped in a `Some` option. If
the function reaches the end of the list without finding a match, `None` is returned.

RustyOptions provides extension methods for most collection types. For more information, see [Collections](collections.md).

Options can also be useful when a value might not exist, for example if it is possible that an exception will
be thrown when you try to construct a value. The following code sample illustrates this.

```csharp
using System.IO;
using RustyOptions;
using static RustyOptions.Option;

Option<FileStream> OpenFile(string filename)
{
    try
    {
        var file = File.Open(filename, FileMode.Create);
        return Some(file);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("An exception occurred opening file '{0}': {1}", 
            filename, ex.Message);
        return None<FileStream>();
    }
}
```

The `OpenFile` function in the previous example returns a `FileStream` option if the file opens successfully,
and `None` if an exception occurs. Depending on the situation, it may not be an appropriate design choice to 
catch an exception rather than allowing it to propagate.

## Null Values

Note that unlike the F# Option type, RustyOptions does not allow `Some(null)` - any attempt to create an option
with a null value will result in `None`.

## Option Properties and Methods

The option type supports the [following properties and methods](../api/RustyOptions.Option-1.yml).

## Converting to Other Types

  - Options can be converted to [Results](result.md) using the 
    [OkOr](../api/RustyOptions.OptionResultExtensions.yml#RustyOptions_OptionResultExtensions_OkOr__2_RustyOptions_Option___0____1_) 
    or [OkOrElse](../api/RustyOptions.OptionResultExtensions.yml#RustyOptions_OptionResultExtensions_OkOrElse__2_RustyOptions_Option___0__Func___1__) extension methods.

  - Options can be converted to `IEnumerable<T>` using the [AsEnumerable](../api/RustyOptions.Option-1.yml#RustyOptions_Option_1_AsEnumerable) method.

  - Options can be converted to `ReadOnlySpan<T>` using the [AsSpan](../api/RustyOptions.Option-1.yml#RustyOptions_Option_1_AsSpan) method.
