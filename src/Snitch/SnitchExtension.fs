namespace Snitch

open System
open System.Collections.Concurrent
open System.Runtime.CompilerServices
open System.Security.Cryptography
open System.Text
open System.Text.Json
open System.Text.RegularExpressions

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
    static member ExtractFirstPath ex =
        let pattern = @"in\s+((?:/[^\s:]+)+:\s*line\s+\d+)" // matches path/to/file:line 1
        let m = Regex.Match(ex, pattern)

        if m.Success then Some m.Groups[1].Value else None

    [<Extension>]
    static member ToHashCode(str: string) =
        use sha = SHA256.Create()
        str |> Encoding.UTF8.GetBytes |> sha.ComputeHash |> Convert.ToHexString

    [<Extension>]
    static member Snitched(ex: Exception) =
        task {
            try
                let app = SnitchServiceLocator.get<ISnitch> ()
                let dict = SnitchServiceLocator.get<ConcurrentDictionary<string, Except>> ()

                let format = ex.ToSlackMessage(app.AppName)

                let path = ex.StackTrace.ExtractFirstPath() |> Option.defaultValue ex.Message // fallback to message if no path found
                let hashCode = path.ToHashCode()

                let except =
                    { Except.Subject = ex.Message
                      Message = format
                      Submitted = false
                      SubmittedAt = None
                      OccurredAt = DateTime.UtcNow }
                // store in dictionary
                dict.TryAdd(hashCode, except) |> ignore

            with _ ->
                () // TODO: swallow the exception for now

            return ()
        }
