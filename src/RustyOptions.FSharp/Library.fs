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
            | (true, value) -> Ok value
            | _ -> Error(x.UnwrapErr())

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

    /// Converts a RustyOptions Option into an F# ValueOption.
    [<Extension>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let AsFSharpOption(x: RustyOptions.Option<'a>) =
        match x.IsSome() with
        | (true, value) -> Some value
        | _ -> None

    /// Converts a RustyOptions Result into an F# Result.
    [<Extension>]
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let AsFSharpResult(x: RustyOptions.Result<'a, 'err>) =
        match x.IsOk() with
        | (true, value) -> Ok value
        | _ -> Error(x.UnwrapErr())

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
        | _ -> RustyOptions.Option.None<'a>()

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
        | _ -> RustyOptions.Option.None<'a>()

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
