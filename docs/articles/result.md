# Results

The `Result<T, TErr>` type lets you write error-tolerant code that can be composed.

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
   the exceptions stack trace. `Result` does not have this overhead.
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

