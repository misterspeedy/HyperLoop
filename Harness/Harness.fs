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
   waveIn.WaveFormat = new WaveFormat(44100, 1) |> ignore
   waveIn.BufferMilliseconds <- 100
   waveIn.NumberOfBuffers <- 2
   waveIn.DeviceNumber <- 0   
   let controller = new Controller(1, listener, waveIn)
   listener.Do("D0")
   System.Threading.Thread.Sleep 1500
   listener.Do("D0")
   printfn "Press a key"
   Console.ReadKey(false) |> ignore
   0 // return an integer exit code
