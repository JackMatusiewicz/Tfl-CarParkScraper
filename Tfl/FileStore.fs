namespace Tfl

module FileStore =
    open Newtonsoft.Json
    open System.IO
    open System
    
    let private serialise (xs : 'a list) : string list  =
        List.map (fun o -> JsonConvert.SerializeObject(o)) xs

    type FileAgentMessage<'t> = | StoreData of 't list

    type FileAgent<'t>(directory : string) =
        let pathToStore = directory
        let agent = MailboxProcessor.Start(fun inbox ->
            let rec loop () = async {
                let! message = inbox.Receive()
                match message with
                | StoreData data ->
                    let jsonData = serialise data
                    let currentDate = DateTime.UtcNow
                    let filePath = Path.Combine(pathToStore,
                                    sprintf "carParkData%s.txt"
                                    <| currentDate.ToString("yyyy-MM-dd"))
                    File.AppendAllLines(filePath, jsonData)
                    return! loop ()
            }
            loop ()
        )

        member this.StoreData (data : 't list) =
            agent.Post(StoreData data)