namespace Snitch

open System
open System.Runtime.CompilerServices
open System.Text.Json

type SnitchExtension =

    [<Extension>]
    static member ToSlackMessage(ex: Exception, appName: string) =
        let message = ex.Message
        let stackTrace = ex.StackTrace
        let header = $"{appName}"
        let message = $"*{message}*\n\n```{stackTrace}```"

        let format =
            {| Blocks =
                [| {| Type = "header"
                      Text = {| Type = "plain_text"; Text = header |} |}
                   {| Type = "section"
                      Text = {| Type = "mrkdwn"; Text = message |} |} |] |}

        let opt = JsonSerializerOptions()
        opt.PropertyNamingPolicy <- JsonNamingPolicy.SnakeCaseLower
        JsonSerializer.Serialize(format, opt)

    [<Extension>]
    static member ToSnitched(ex: Exception, app: Slack) =
        task {
            let format = ex.ToSlackMessage(app.AppName)
            do! app.Submit(format)
        }
