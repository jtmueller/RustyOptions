# Results

The `Result<T, TErr>` type lets you write error-tolerant code that can be composed.

```csharp
using RustyOptions;
using static RustyOptions.Result;

// Defaults to an error type of string
Result<int, string> ok1 = Ok(42);
// Defaults to an error type of Exception
Result<int, Exception> ok2 = OkExn(42);
// fully-specified types
Result<int, MyCustomErrType> ok3 = Ok<int, MyCustomErrType>(42);

// Error type is string, but you must specify the Ok type
Result<int, string> err1 = Err<int>("oops");
// Error type is Exception, but you must specify the Ok type.
Result<int, Exception> err2 = Err<int>(new Exception("oops"));
// fully-specified types
Result<int, MyCustomErrType> err3 = Err<int, MyCustomErrType>(new MyCustomErrType("oops"));
```

## Remarks

The `Result` type is used for operations that might not succeed, and need to return a reason if the operation failed.
In other words, error handling.

Because C# is fundamentally based on exceptions for error handling, it is not advisable to attempt to write a large program
that uses `Result` exclusively for all error handling. The lack of stack traces will make debugging difficult, and it's not
possible to completely eliminate all need for try/catch statements.

Where `Result` shines is in specific, isolated use-cases where operations are expected to fail, and we need to know why the
operation failed, but do not want to pay the cost of constructing a stack-trace for an Exception. One example would be input
validation.

Results have both advantages and disadvantages when compared to exception-based error handling.

### Advantages

 - Throwing an exception for a routine, non-exceptional failure (such as input validation) is expensive, mainly due to creating
   the exception's stack trace. `Result` does not have this overhead.
 - If you want to avoid using exceptions in such cases, you need to manually track three things:
     - Did the operation succeed?
     - If so, what is the return value?
     - If not, why not?
 - The `Result` type tracks exactly this information, in a way that is easily composable with other results.
 - `Result` does not force you to use any specific type to indicate an error. Errors can be instances of `Exception` but they
   can also be strings or any custom type you desire.

### Disadvantages

 - It's easier to accidentally ignore a `Result` in the `Err` state than it is to accidentally ignore an exception.
 - Exceptions have stack traces, whereas `Result` does not. This can make errors more difficult to track down
   if you have many deeply-nested Result-returning functions.

## Example

```csharp
using RustyOptions;
using static RustyOptions.Result;

// Define a simple type which has fields that can be validated
record Request(string Name, string Email);

// Define some logic for what defines a valid name.
// Generates a Result which is an Ok if the name validates;
// otherwise, it generates a Result which is an Err.
Result<Request, string> ValidateName(Request req) =>
    req.Name switch
    {
        null => Err<Request>("No name found."),
        "" => Err<Request>("Name is empty."),
        "bananas" => Err<Request>("Bananas is not a name."),
        _ => Ok(req)
    };

// Similarly, define some email validation logic.
Result<Request, string> ValidateEmail(Request req) =>
    req.Email switch
    {
        null => Err<Request>("No email found."),
        "" => Err<Request>("Email is empty."),
        var s when s.EndsWith("bananas.com") => Err<Request>("No email from bananas.com is allowed."),
        _ => Ok(req)
    };

// Compose the name/email validation functions
Result<Request, string> ValidateRequest(Result<Request, string> reqResult) =>
    reqResult.AndThen(ValidateName).AndThen(ValidateEmail);

void Test()
{
    // Now, create and validate a Request and check the result.
    var req1 = new Request("Phillip", "phillip@contoso.biz");
    var res1 = ValidateRequest(Ok(req1));
    res1.Match(
        onOk: req => Console.WriteLine($"My request was valid! Name: {req.Name} Email: {req.Email}"),
        onErr: e => Console.WriteLine($"Error: {e}")
    );
    // Prints: "My request was valid! Name: Phillip Email: phillip@contoso.biz"

    var req2 = new Request("Phillip", "phillip@bananas.com");
    var res2 = ValidateRequest(Ok(req2));
    res2.Match(
        onOk: req => Console.WriteLine($"My request was valid! Name: {req.Name} Email: {req.Email}"),
        onErr: e => Console.WriteLine($"Error: {e}")
    );
    // Prints: "Error: No email from bananas.com is allowed."
}

```

## Converting from Exceptions to Results

You can use `Result.Try` to call a function that might throw an exception, returning either the return value or the exception wrapped in a `Result`.

```csharp
var res = Result.Try(MethodMaybeThrows);

int x = 5;
int y = 10;
var res2 = Result.Try(() => MethodWithArgsMaybeThrows(x, y));
```

## Converting to Other Types

`Result<T, TErr>` can be converted to `Option<T>` with the [Ok](../api/RustyOptions.OptionResultExtensions.yml#RustyOptions_OptionResultExtensions_Ok__2_RustyOptions_Result___0___1__) extension method.

`Result<T, TErr>` can be converted to `Option<TErr>` with the [Err](../api/RustyOptions.OptionResultExtensions.yml#RustyOptions_OptionResultExtensions_Err__2_RustyOptions_Result___0___1__) extension method.

`Result<T, TErr>` can be converted to `IEnumerable<T>` with the [AsEnumerable](../api/RustyOptions.Result-2.yml#RustyOptions_Result_2_AsEnumerable) method.

`Result<T, TErr>` can be converted to `ReadOnlySpan<T>` with the [AsSpan](../api/RustyOptions.Result-2.yml#RustyOptions_Result_2_AsSpan) method.
