namespace Snitch

open System.Runtime.ExceptionServices
open Microsoft.AspNetCore.Http

type SnitchMiddleware(next: RequestDelegate) =
    
    member this.InvokeAsync(context: HttpContext) =
        task { 
            try
                do! next.Invoke(context)
            with
            | ex ->
                do! ex.Snitched()
                ExceptionDispatchInfo.Capture(ex).Throw()
                
                return ()
        }
        
