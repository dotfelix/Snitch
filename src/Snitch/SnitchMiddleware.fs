namespace Snitch

open System.Runtime.ExceptionServices
open Microsoft.AspNetCore.Http

type SnitchMiddleware(next: RequestDelegate, app: Slack) =
    
    member this.InvokeAsync(context: HttpContext) =
        task { 
            try
                do! next.Invoke(context)
            with
            | ex ->
                do! ex.ToSnitched(app)
                ExceptionDispatchInfo.Capture(ex).Throw()
                
                return ()
        }
        
