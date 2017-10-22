namespace Tfl

module Option =
    type OptionBuilder () =
        member this.Return(x) = Some x
        member this.ReturnFrom(x) = x
        member this.Bind(x, f) =
            match x with
            | Some a -> f a
            | None -> None
    let opt = OptionBuilder()

