namespace Snitch

open System
open System.Collections.Concurrent
open Microsoft.Extensions.Hosting

type SnitchService
    (
        app: ISnitch,
        dict: ConcurrentDictionary<int, string>
    ) =
    inherit BackgroundService()

    override this.ExecuteAsync ct =
        task {
            while not ct.IsCancellationRequested do
                try
                    () // do nothing for now
                with
                | :? OperationCanceledException -> () // do nothing for now
                | _ex -> () // do nothing for now
        }
