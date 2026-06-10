namespace Snitch

open System
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Hosting

type SnitchService(app: ISnitch, dict: ConcurrentDictionary<string, Except>) =
    inherit BackgroundService()

    let processExcept now except =
        task {
            if not except.Submitted then
                let! submitted = app.Submit(except.Message)

                if submitted then
                    let updatedExcept =
                        { except with
                            Submitted = true
                            SubmittedAt = Some DateTime.UtcNow }

                    return updatedExcept, false
                else
                    // Keep retrying on future passes
                    return except, false
            else
                match except.SubmittedAt with
                | Some submittedAt ->
                    let submittedDiff = now - submittedAt

                    if submittedDiff.TotalMinutes > 15.0 then // TODO: move to config
                        return except, true
                    else
                        return except, false

                | None ->
                    // Inconsistent state; remove it
                    return except, true
        }

    override _.ExecuteAsync(ct: CancellationToken) =
        task {
            while not ct.IsCancellationRequested do
                try
                    let snapshot = dict |> Seq.map (fun kvp -> kvp.Key, kvp.Value) |> Seq.toList

                    for key, except in snapshot do
                        let now = DateTime.UtcNow
                        let diff = now - except.OccurredAt

                        if diff.TotalMinutes > 1.0 then
                            let! updatedExcept, shouldRemove = processExcept now except

                            if shouldRemove then
                                dict.TryRemove(key) |> ignore
                            else
                                dict.AddOrUpdate(key, updatedExcept, fun _ _ -> updatedExcept) |> ignore

                with :? OperationCanceledException ->
                    ()

                do! Task.Delay(TimeSpan.FromMinutes(1), ct)
        }
