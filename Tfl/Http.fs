namespace Tfl

module HttpRequest =
    open System.Net

    let get (url : string) =
        use client = new WebClient()
        client.DownloadString(url)