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
   listener.Do("D0")
   printfn "Recording"
   System.Threading.Thread.Sleep 5000
   listener.Do("D0")
   printfn "Playing"

   let waveOut = new WaveOutEvent()
   let player = new Player(waveOut)
   waveOut.DeviceNumber <- 0

   player.Play controller.Recorders.[0].Buffer
   printfn "%A" player.WaveOut.PlaybackState
   System.Threading.Thread.Sleep 100
   printfn "%A" player.WaveOut.PlaybackState
   System.Threading.Thread.Sleep 3000
   printfn "%A" player.WaveOut.PlaybackState
   printfn "Press a key"
   Console.ReadKey(false) |> ignore
   0 // return an integer exit code
