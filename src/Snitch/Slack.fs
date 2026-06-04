namespace Snitch

open System
open System.Net.Http
open System.Text

type Slack(client: HttpClient, appName: string, url: string) =
    member val Client = client with get, set 
    
    interface ISnitch with
        member this.AppName = appName
        member this.Submit message =
            task {
                let payload = new StringContent(message, Encoding.UTF8, "application/json")
                let baseAddress = Uri(url)
                let! response = client.PostAsync(baseAddress, payload)
                
                if not response.IsSuccessStatusCode then
                    let! error = response.Content.ReadAsStringAsync()
                    error |> ignore // TODO send to log
                    return false
                else
                    return true    
            }