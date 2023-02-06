# Collections

RustyOptions provides extension methods that return options for collection operations that might not produce a value:
 - `TryXXX` methods have `XXXOrNone` equivalents.
 - `XXXOrDefault` methods have `XXXOrNone` equivalents.

In addition, the `.Values()` extension method will pull all the values that exist out of a collection of `Option<T>` or `Result<T, TErr>`,
while the `.Errors()` extension method will pull all of the errors from a collection of `Result<T, TErr>`.

## Supported Collections
 - Any type implementing `IEnumerable<T>` (First, Last, Single, ElementAt)
 - `IDictionary<TKey, TValue>`, `IReadOnlyDictionary<TKey, TValue>`
 - `Stack<T>`, `ImmutableStack<T>`, `ConcurrentStack<T>`
 - `Queue<T>`, `PriorityQueue<T, TPriority>`, `ImmutableQueue<T>`, `ConcurrentQueue<T>`
 - `HashSet<T>`, `SortedSet<T>`, `ImmutableHashSet<T>`, `ImmutableSortedSet<T>`
 - `IProducerConsumerCollection<T>`, `ConcurrentBag<T>`

For details, see the [API Documentation](../api/RustyOptions.OptionCollectionExtensions.yml).
