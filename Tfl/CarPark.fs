namespace Tfl

type Bay = {
    bayCount : uint32
    free : uint32
    occupied : uint32
    bayType : string
}

type CarParkInfo = {
    bays : Bay list
    name : string
    carParkDetailsUrl : string
    id : string
}

type CarParkDatabaseRecord = {
    bayCount : uint32
    free : uint32
    occupied : uint32
    bayType : string
    carParkId : string
    timestamp : System.DateTime
}

type CredentialUrlSegment = private Creds of string

module CarPark =
    open Newtonsoft.Json
    open System
    open Tfl.Option

    let constructCredentialsUrlSegment (apiKey : string) (appId : string) : CredentialUrlSegment =
        sprintf "app_id=%s&app_key=%s" appId apiKey |> Creds

    let private makePair (a : 'a) (b : 'b) = a, b

    let constructDatabaseRecords ((timestamp, carParkInfo) : DateTime * CarParkInfo) : CarParkDatabaseRecord list =
        let innerConstruct (timestamp : DateTime) (id : string) (bay : Bay) =
            {bayCount = bay.bayCount; free = bay.free; occupied = bay.occupied;
                bayType = bay.bayType; timestamp = timestamp; carParkId = id}
        List.map (innerConstruct timestamp (carParkInfo.id)) carParkInfo.bays

    let getCarParkInfo ((Creds creds) : CredentialUrlSegment) : ((DateTime * CarParkInfo) list) option =
        try
            let url = "https://api.tfl.gov.uk/Occupancy/CarPark"
            let urlWithCreds = url + "/?" + creds
            let body = HttpRequest.get urlWithCreds
            JsonConvert.DeserializeObject<CarParkInfo list>(body)
                |> List.map (makePair DateTime.UtcNow)
                |> Some
        with
            | _ -> None

    let constructNewDataStore (creds : CredentialUrlSegment) : (CarParkDatabaseRecord list) option = opt {
        let! data = getCarParkInfo creds
        let records = data |> List.map constructDatabaseRecords
                           |> List.concat
        return records
    }
