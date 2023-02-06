# Formatting Options and Results

RustyOptions types implement both `IFormattable` and `ISpanFormattable` for efficient use in string-building. Any format strings
are passed through to the contained type.

```csharp
Assert.Equal("Some(4,200.00)", Some(4200).ToString("n2", CultureInfo.InvariantCulture));
Assert.Equal("None", None<int>().ToString("n2", CultureInfo.InvariantCulture));

Assert.Equal("Ok(4,200.00)", Ok(4200).ToString("n2", CultureInfo.InvariantCulture));
Assert.Equal("Err(oops)", Err<int>("oops").ToString("n2", CultureInfo.InvariantCulture));
```
