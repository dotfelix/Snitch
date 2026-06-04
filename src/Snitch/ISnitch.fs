namespace Snitch

open System
open System.Threading.Tasks

type ISnitch =
    abstract member Submit : string -> Task<bool>
    abstract member AppName : string
    
    
type Except = {
    Subject: string
    Message: string
    Submitted: bool
    SubmittedAt: DateTime option
    OccurredAt: DateTime
}    