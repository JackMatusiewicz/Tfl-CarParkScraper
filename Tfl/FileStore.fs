namespace Tfl

module FileStore =
    open Newtonsoft.Json
    open System.IO
    
    let private serialise (xs : 'a list) : string list  =
        List.map (fun o -> JsonConvert.SerializeObject(o)) xs

    type FileAgentMessage<'t> = | StoreData of 't list

    type FileAgent<'t>(path : string) =
        let filePath = path
        let agent = MailboxProcessor.Start(fun inbox ->
            let loop () = async {
                let! message = inbox.Receive()
                match message with
                | StoreData data ->
                    let jsonData = serialise data
                    File.AppendAllLines(filePath, jsonData)
            }
            loop ()
        )

        member this.StoreData (data : 't list) =
            agent.Post(StoreData data)

