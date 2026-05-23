namespace Snitch

open System
open System.Net.Http
open System.Text

type Slack(client: HttpClient, appName: string, url: string) =
    member val Client = client with get, set
    member val AppName = appName with get, set

    member _.Submit(formatedMessage: string) =
        task {
            let payload = new StringContent(formatedMessage, Encoding.UTF8, "application/json")
            let baseAddress = Uri(url)
            let! response = client.PostAsync(baseAddress, payload)
            
            if not response.IsSuccessStatusCode then
                let! error = response.Content.ReadAsStringAsync()
                printfn "Failed to send to Slack: %s" error
        }
        
