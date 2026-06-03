namespace Snitch

open System
open Microsoft.Extensions.DependencyInjection

module SnitchServiceLocator =

    let mutable private provider: IServiceProvider option = None

    let initialize sp = provider <- Some sp

    let get<'t> () =
        provider.Value.GetRequiredService<'t>()

