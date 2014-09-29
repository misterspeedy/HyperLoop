namespace HyperLoop

open System
open System.IO.Ports

/// Listens for data on the specified port and triggers its OnCommand event
/// whenever a complete command comes through (terminated with a \n).
type Listener(portName : string, speed : int) as this =
    let _port = new SerialPort(portName, speed)
    let _onCommand = new Event<ListenerCommandDelegate, ListenerCommandArgs>()
    let mutable _buffer = ""

    let receiveData() =
        let input = _port.ReadExisting()
        _buffer <- _buffer+input
        if _buffer.Contains("\n") then
            let parts = _buffer.Split([|'\n'|])
            let command = new ListenerCommandArgs(parts.[0])
            _onCommand.Trigger(this, command)
            _buffer <- parts.[1]

    do
        _port.DataReceived.Add(fun _ -> receiveData())
        _port.Open()

    interface IDisposable with
        member __.Dispose() =
            _port.Close()

    interface IListener with
        [<CLIEvent>]
        member this.OnCommand = _onCommand.Publish
