# F# Compatibility

The F# language has its own built-in `Option`, `ValueOption`, and `Result` types, and both language features and
utility functions to make these types ergonomic to work with in the context of F#.

There would be little value in extensive F# support for RustyOptions. Similarly, while the F# types can be used
from C# by referencing the FSharp.Core namespace, they are not convenient to use from C#, and the FSharp.Core
dependency is fairly heavy-weight for just these types.

RustyOptions is therefore designed to be used from C#. However, it provides the `RustyOptions.FSharp` nuget package,
which provides convenient methods for converting between RustyOptions Option/Result types, and F# Option/Result types.
If you are working with a mixture of F# and C# code, RustyOptions can ease the interop between these two languages:

  - C# code receiving values from F# code can use `FromFSharpXXX` extension methods to convert F# Options or Results
    into RustyOptions equivalents that are easier to work with from C#.
  - C# code passing values to F# can use RustyOptions types for ease of working with in C#, then use `AsFSharpXXX` extension
    methods to convert to the equivalent F# types that the F# code expects.
  - F# code receiving values from C# that use RustyOptions types can use `Option.ofRustyOption` or `ValueOption.ofRustyOption` or
    `Result.ofRustyResult` to convert to types that are more convenient to use in F#.
  - F# code passing values to C# can use `Option.toRustyOption` or `ValueOption.toRustyOption` or `Result.toRustyResult` to
    convert to any RustyOptions types expected by C# code.

