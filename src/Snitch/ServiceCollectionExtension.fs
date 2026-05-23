namespace Snitch

open System
open System.Net.Http
open System.Runtime.CompilerServices
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection

type ServiceCollectionExtension =
    
    [<Extension>]
    static member AddSnitch (services: IServiceCollection, appName: string) =
        let serviceProvider = services.BuildServiceProvider()
        let config = serviceProvider.GetRequiredService<IConfiguration>()
        
        let slackHookUrl = config["SLACK_HOOK_URL"] // Slack webhook URL, throw exception if not found 
        
        let httpClient = new HttpClient() 
        let app = Slack(httpClient, appName, slackHookUrl)

        services.AddSingleton(app) |> ignore

        services
    
    [<Extension>]
    static member UseSnitch (app: IApplicationBuilder) =
        app.UseMiddleware<SnitchMiddleware>()