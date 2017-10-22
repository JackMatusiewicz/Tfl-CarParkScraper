namespace Tfl

type Args = {
    AppId : string
    AppKey : string
}

module Argument =
    let private emptyArgs = {AppId = ""; AppKey = ""}

    let private addArg (arg : string) (args : Args) : Args =
        match arg with
        | _ when arg.StartsWith("--appId=") ->
                let appId = arg.Replace("--appId=", "")
                {args with AppId = appId}
        | _ when arg.StartsWith("--appKey=") ->
                let appKey = arg.Replace("--appKey=", "")
                {args with AppKey = appKey}
        | _ -> args

    let parse (args : string[]) : Args =
        Array.foldBack addArg args emptyArgs

