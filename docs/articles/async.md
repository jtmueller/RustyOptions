# Async

The `RustyOptions.Async` namespace provides extension methods for `IAsyncEnumerable<T>`
as well as async versions of RustyOptions API methods like `Map`, `OrElse`, `AndThen`.

## IAsyncEnumerable<T>

RustyOptions provides `FirstOrNoneAsync` for any async enumerable, as well as `Values` and `Errors` methods
for async enumerables that return `Option<T>` or `Result<T, TErr>`.

## Async Option/Result Methods

RustyOptions provides asynchronous versions of most Option/Result API methods. There are 8 overloads of each
method, for the various combinations of `Task` and `ValueTask` plus tasks that return an Option/Result vs 
Option/Result objects that return a task. Because there are so many overloads, they are found in the 
`RustyOptions.Async` namespace so that they don't clutter intellisense for non-async code.
