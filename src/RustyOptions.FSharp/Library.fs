namespace RustyOptions.FSharp

open System.Runtime.CompilerServices

/// This module provides F# extension methods on RustyOptions types.
[<AutoOpen>]
module TypeExtensions =

    type RustyOptions.Option<'a> with
        /// Converts a RustyOptions Option into an F# ValueOption.
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member x.AsFSharpValueOption() =
            match x.IsSome() with
            | (true, value) -> ValueSome value
            | _ -> ValueNone

        /// Converts a RustyOptions Option into an F# Option.
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member x.AsFSharpOption() =
            match x.IsSome() with
            | (true, value) -> Some(value)
            | _ -> None

    type RustyOptions.Result<'a, 'err> with
        /// Converts a RustyOptions Result into an F# Result.
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member x.AsFSharpResult() =
            match x.IsOk() with
            | (true, value) -> Ok(value)
            | _ -> Error(x.UnwrapErr())

    type RustyOptions.Unit with
        /// Converts a RustyOptions Unit into an F# unit.
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member _.AsFSharpUnit() : unit = ()

    type Microsoft.FSharp.Core.Unit with
        /// Converts an F# unit into a RustyOptions Unit.
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member _.AsRustyUnit() = RustyOptions.Unit.Default

#if NET7_0_OR_GREATER
    type RustyOptions.NumericOption<'a when 'a : struct and 'a :> System.ValueType and 'a : (new : unit -> 'a) and 'a :> System.Numerics.INumber<'a>> with

        /// Converts a RustyOptions NumericOption into an F# ValueOption.
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member x.AsFSharpValueOption() =
            match x.IsSome() with
            | (true, value) -> ValueSome value
            | _ -> ValueNone

        /// Converts a RustyOptions NumericOption into an F# Option.
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member x.AsFSharpOption() =
            match x.IsSome() with
            | (true, value) -> Some(value)
            | _ -> None
#endif

/// This module provides C# extension methods on RustyOptions types.
[<Extension>]
module CSharpTypeExtensions =

    /// Converts a RustyOptions Option into an F# ValueOption.
    [<Extension>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let AsFSharpValueOption(x: RustyOptions.Option<'a>) =
        match x.IsSome() with
        | (true, value) -> ValueSome value
        | _ -> ValueNone

    // We have to use FromFSharpValueOption and FromFSharpOption because
    // F# doesn't allow overloads in this circumstance, so ToRustyOption will not work.

    /// Converts an F# ValueOption into a RustyOptions Option.
    [<Extension>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let FromFSharpValueOption(x: 'a voption) =
        match x with
        | ValueSome(value) -> RustyOptions.Option.Some(value)
        | _ -> RustyOptions.Option<'a>.None

    /// Converts a RustyOptions Option into an F# Option.
    [<Extension>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let AsFSharpOption(x: RustyOptions.Option<'a>) =
        match x.IsSome() with
        | (true, value) -> Some value
        | _ -> None

    /// Converts an F# Option into a RustyOptions Option.
    [<Extension>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let FromFSharpOption(x: 'a option) =
        match x with
        | Some(value) -> RustyOptions.Option.Some(value)
        | _ -> RustyOptions.Option<'a>.None

    /// Converts a RustyOptions Result into an F# Result.
    [<Extension>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let AsFSharpResult(x: RustyOptions.Result<'a, 'err>) =
        match x.IsOk() with
        | (true, value) -> Ok value
        | _ -> Error(x.UnwrapErr())

    /// Converts an F# Result into a RustyOptions Result.
    [<Extension>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let FromFSharpResult(x: Result<'a, 'err>) =
        match x with
        | Ok(value) -> RustyOptions.Result.Ok<'a, 'err>(value)
        | Error(err) -> RustyOptions.Result.Err<'a, 'err>(err)

    /// Converts a RustyOptions Unit Result into an F# unit Result.
    [<Extension>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let AsFSharpUnitResult(x: RustyOptions.Result<RustyOptions.Unit, 'err>) =
        match x.IsOk() with
        | (true, _) -> Ok ()
        | _ -> Error(x.UnwrapErr())

    /// Converts an F# unit Result into a RustyOptions Unit Result.
    [<Extension>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let FromFSharpUnitResult(x: Result<unit, 'err>) =
        match x with
        | Ok _ -> RustyOptions.Result.Ok<RustyOptions.Unit, 'err>(RustyOptions.Unit.Default)
        | Error(err) -> RustyOptions.Result.Err<RustyOptions.Unit, 'err>(err)

    // NOTE: We can't have an AsFSharpUnit method for C#, because C# interprets
    // a unit-returning F# function as if it's a void-returning C# function.

#if NET7_0_OR_GREATER
/// This module provides C# extension methods on RustyOptions numeric types.
[<Extension>]
module CSharpNumericTypeExtensions =

    /// Converts a RustyOptions Option into an F# ValueOption.
    [<Extension>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let AsFSharpValueOption(x: RustyOptions.NumericOption<'a>) =
        match x.IsSome() with
        | (true, value) -> ValueSome value
        | _ -> ValueNone

    /// Converts a RustyOptions Option into an F# ValueOption.
    [<Extension>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let AsFSharpOption(x: RustyOptions.NumericOption<'a>) =
        match x.IsSome() with
        | (true, value) -> Some value
        | _ -> None
#endif

module Option =

    /// Converts a RustyOptions Option into an F# Option.
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let ofRustyOption (x: RustyOptions.Option<'a>) =
        match x.IsSome() with
        | (true, value) -> Some value
        | _ -> None

    /// Converts an F# Option into a RustyOptions Option.
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let toRustyOption (x: 'a option) =
        match x with
        | Some(value) -> RustyOptions.Option.Some(value)
        | _ -> RustyOptions.Option<'a>.None

#if NET7_0_OR_GREATER
    /// Converts a RustyOptions Option into an F# Option.
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let ofRustyNumericOption (x: RustyOptions.NumericOption<'a>) =
        match x.IsSome() with
        | (true, value) -> Some value
        | _ -> None

    /// Converts an F# Option into a RustyOptions Option.
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let toRustyNumericOption (x: 'a option) =
        match x with
        | Some(value) -> RustyOptions.NumericOption.Some(value)
        | _ -> RustyOptions.NumericOption.None<'a>()
#endif

module ValueOption =

    /// Converts a RustyOptions Option into an F# ValueOption.
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let ofRustyOption (x: RustyOptions.Option<'a>) =
        match x.IsSome() with
        | (true, value) -> ValueSome value
        | _ -> ValueNone

    /// Converts an F# Option into a RustyOptions Option.
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let toRustyOption (x: 'a voption) =
        match x with
        | ValueSome(value) -> RustyOptions.Option.Some(value)
        | _ -> RustyOptions.Option<'a>.None

#if NET7_0_OR_GREATER
    /// Converts a RustyOptions Option into an F# Option.
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let ofRustyNumericOption (x: RustyOptions.NumericOption<'a>) =
        match x.IsSome() with
        | (true, value) -> ValueSome value
        | _ -> ValueNone

    /// Converts an F# Option into a RustyOptions Option.
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let toRustyNumericOption (x: 'a voption) =
        match x with
        | ValueSome(value) -> RustyOptions.NumericOption.Some(value)
        | _ -> RustyOptions.NumericOption.None<'a>()
#endif

module Result =

    /// Converts a RustyOptions Result into an F# Result.
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let ofRustyResult (x: RustyOptions.Result<'a, 'err>) =
        match x.IsOk() with
        | (true, value) -> Ok value
        | _ -> Error(x.UnwrapErr())

    /// Converts an F# Result into a RustyOptions result.
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let toRustyResult (x: Result<'a, 'err>) =
        match x with
        | Ok(value) -> RustyOptions.Result.Ok<'a, 'err>(value)
        | Error(err) -> RustyOptions.Result.Err<'a, 'err>(err)

    /// Converts a RustyOptions Unit Result into an F# unit Result.
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let ofRustyUnitResult (x: RustyOptions.Result<RustyOptions.Unit, 'err>) =
        match x.IsOk() with
        | (true, _) -> Ok ()
        | _ -> Error(x.UnwrapErr())

    /// Converts an F# unit Result into a RustyOptions Unit Result.
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let toRustyUnitResult (x: Result<unit, 'err>) =
        match x with
        | Ok(_) -> RustyOptions.Result.Ok<RustyOptions.Unit, 'err>(RustyOptions.Unit.Default)
        | Error(err) -> RustyOptions.Result.Err<RustyOptions.Unit, 'err>(err)
