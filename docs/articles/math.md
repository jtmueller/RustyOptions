# Generic Math

_NOTE: Generic Math is supported by .NET 7 and above only. This feature is not available in .NET 6._

.NET 7 introduces new [math-related generic interfaces](https://learn.microsoft.com/en-us/dotnet/standard/generics/math) to the base class library. RustyOptions provides the `NumericOption<T>` type to support this. `NumericOption` can store any struct that implements the `INumber<T>` interface, and allows you to transparently perform math operations on these optional values.

This works similarly to `Nullable<T>` which also [allows you to perform math operations on contained values](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types#lifted-operators). Such operations produce `null` if one or both operands are `null` in the case of `Nullable<T>`, and they produce `None` if one or both operands are `None` in the case of `NumericOption<T>`.

The difference is that this functionality in `Nullable<T>` depends on compiler support, which is not available to third-party types. This is why you can only do math with options in the .NET 7+ version of RustyOptions.

`NumericOption<T>` supports implicit conversion from `T` which means you can mix, for example, `NumericOption<int>` and `int` in the same expression, and it will just work.

## Example

```csharp
using RustyOptions;
using static RustyOptions.NumericOption;

var fifteen = Some(3) * 5;
// returns Some(15)

var none = Some(3) * None<int>();
// returns None typed to int

// You can even use Option<int> as the index value in a for loop!
for (var i = Some(0); i < 5; i++)
{
    // If you set i to None inside the loop,
    // the loop will exit when the current iteration completes,
    // as None is not less than 5.
    i = None<int>();
}
```

As you can see from the last example above, comparison operators such as `<` and increment operators like `++` also work with `NumericOption<T>`, even when comparing an option to a concrete number.

`NumericOption<T>` can be implicitly converted to and from `Option<T>` provided that the contained type is compatible with `NumericOption<T>`.

This support works for any built-in type that implements `INumber<T>`, any custom type that implements `INumber<T>`, and any future built-in types that implement `INumber<T>`. Enjoy!
