namespace Snitch.Example

#nowarn "20"

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Snitch

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddConnections()
        builder.Services.AddSnitch("Snitch.Example")

        let app = builder.Build()

        app.UseSnitch()
        // app.MapControllers()

        app.MapGet(
            "/default",
            Func<HttpContext, IActionResult>(fun _ ->
                let _ = 1 / 0 // <-- exception here
                OkResult())
        )

        app.MapGet(
            "/handled",
            Func<HttpContext, IActionResult>(fun ctx -> 
                try
                    let rst = 1 / 0 // <-- exception here but captured
                    rst |> ignore
                with ex ->
                    ex.ToSnitched() |> ignore
                OkResult())
        )

        app.Run()

        exitCode
