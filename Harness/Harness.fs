open System
open NAudio.Wave
open HyperLoop

type private DummyListener() =
    let _onCommand = new Event<ListenerCommandDelegate, ListenerCommandArgs>()
    member this.Do cmd = 
        _onCommand.Trigger(this, new ListenerCommandArgs(cmd))
    interface IListener with
        [<CLIEvent>]
        member this.OnCommand = _onCommand.Publish

[<EntryPoint>]
let main argv = 
   let listener = new DummyListener()
   let waveIn = new NAudio.Wave.WaveInEvent()
   // TODO magic numbers
   waveIn.WaveFormat <- new WaveFormat(44100, 1)
   waveIn.BufferMilliseconds <- 100
   waveIn.NumberOfBuffers <- 2
   waveIn.DeviceNumber <- 0   
   let controller = new Controller(1, listener, waveIn)
   printfn "Press a key to record"
   Console.ReadKey(false) |> ignore
   listener.Do("D0")
   printfn "Recording"
   printfn "Press a key to loop"
   Console.ReadKey(false) |> ignore
   listener.Do("D0")
   printfn "Playing"

   let waveOut = new WaveOutEvent()
   let player = new Player(waveOut)
   waveOut.DeviceNumber <- 0

   player.PlayLooped controller.Recorders.[0].Buffer
   printfn "Press a key to exit"
   Console.ReadKey(false) |> ignore
   0 // return an integer exit code
