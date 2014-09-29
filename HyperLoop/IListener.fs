namespace HyperLoop

open System

type ListenerCommandArgs(command : string) =
    inherit EventArgs()
    member __.Command = command

type ListenerCommandDelegate = delegate of obj * ListenerCommandArgs -> unit

type IListener =
    [<CLIEvent>]
    abstract member OnCommand : IEvent<ListenerCommandDelegate, ListenerCommandArgs>

