namespace Tfl

module Program =
    open System
    open FileStore
    open System.Net
    open Tfl.CarPark

    ServicePointManager.SecurityProtocol <- SecurityProtocolType.Tls12
    ServicePointManager.Expect100Continue <- true;

    [<Literal>]
    let fiveMinutesInMillis = 300000 

    let rec updateStore (agent : FileAgent<CarParkDatabaseRecord>) (creds : CredentialUrlSegment) : Async<unit> = async {
        let data = constructNewDataStore creds
        match data with
        | Some records ->
            agent.StoreData records
            printfn "Updated at %s" <| DateTime.UtcNow.ToString("O")
            do! Async.Sleep fiveMinutesInMillis
            return! updateStore agent creds
        | None ->
            printfn "Error contacting TFL data source"
            do! Async.Sleep fiveMinutesInMillis
            return! updateStore agent creds
    }

    [<EntryPoint>]
    let main argv = 
        let args = Argument.parse argv
        let creds = constructCredentialsUrlSegment args.AppKey args.AppId
        let agent = FileAgent<CarParkDatabaseRecord>("carParkData.txt")

        updateStore agent creds |> Async.RunSynchronously

        0