module Tests

open Xunit
open RustyOptions.FSharp

[<Fact>]
let ``Can convert with extension methods`` () =
    let some = RustyOptions.Option.Some(42)
    let none = RustyOptions.Option.None<int>()
    let ok = RustyOptions.Result.Ok<int, string>(42)
    let err = RustyOptions.Result.Err<int>("oops")

    Assert.Equal(Some(42), some.AsFSharpOption());
    Assert.Equal(None, none.AsFSharpOption());

    Assert.Equal(ValueSome(42), some.AsFSharpValueOption());
    Assert.Equal(ValueNone, none.AsFSharpValueOption());

    Assert.Equal(Ok(42), ok.AsFSharpResult());
    Assert.Equal(Error("oops"), err.AsFSharpResult());

[<Fact>]
let ``Can convert Option with module functions`` () =
    let some = Some 42
    let none = None

    let rustySome = some |> Option.toRustyOption
    let rustyNone = none |> Option.toRustyOption

    let someConverted = rustySome |> Option.ofRustyOption
    let noneConverted = rustyNone |> Option.ofRustyOption

    Assert.Equal(RustyOptions.Option.Some(42), rustySome)
    Assert.Equal(RustyOptions.Option.None<int>(), rustyNone)

    Assert.Equal(some, someConverted)
    Assert.Equal(none, noneConverted)

[<Fact>]
let ``Can convert ValueOption with module functions`` () =
    let some = ValueSome 42
    let none = ValueNone

    let rustySome = some |> ValueOption.toRustyOption
    let rustyNone = none |> ValueOption.toRustyOption

    let someConverted = rustySome |> ValueOption.ofRustyOption
    let noneConverted = rustyNone |> ValueOption.ofRustyOption

    Assert.Equal(RustyOptions.Option.Some(42), rustySome)
    Assert.Equal(RustyOptions.Option.None<int>(), rustyNone)

    Assert.Equal(some, someConverted)
    Assert.Equal(none, noneConverted)

[<Fact>]
let ``Can convert Result with module functions`` () =
    let ok = Ok 42
    let err = Error "oops"

    let rustyOk = ok |> Result.toRustyResult
    let rustyErr = err |> Result.toRustyResult

    let okConverted = rustyOk |> Result.ofRustyResult
    let errConverted = rustyErr |> Result.ofRustyResult

    Assert.Equal(RustyOptions.Result.Ok<int>(42), rustyOk)
    Assert.Equal(RustyOptions.Result.Err<int>("oops"), rustyErr)

    Assert.Equal(ok, okConverted)
    Assert.Equal(err, errConverted)
