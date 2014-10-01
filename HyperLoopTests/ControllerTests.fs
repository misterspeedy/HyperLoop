[<NUnit.Framework.TestFixture>]
module ControllerTests 

open System
open NUnit.Framework
open FsUnit
open HyperLoop
open Moq
open NAudio.Wave

type private DummyListener() =
    let _onCommand = new Event<ListenerCommandDelegate, ListenerCommandArgs>()
    member this.Do cmd = 
        _onCommand.Trigger(this, new ListenerCommandArgs(cmd))
    interface IListener with
        [<CLIEvent>]
        member this.OnCommand = _onCommand.Publish

let MockWaveIn() =
   let waveIn = new Mock<IWaveIn>()
   waveIn.Object

// Invalid lengths:
[<TestCase("")>]
[<TestCase("X")>]
[<TestCase("XYZ")>]
// Invalid switch number:
[<TestCase("D2")>]
// Invalid verb:
[<TestCase("X0")>]
let ``An invalid command causes an exception``(command : string) = 
    let listener = DummyListener()
    let controller = new Controller(1, listener, MockWaveIn())
    (fun () -> listener.Do command) |> should throw typeof<ArgumentException>

[<TestCase("D0", 1)>]
[<TestCase("D0", 2)>]
[<TestCase("D1", 2)>]
[<TestCase("D2", 3)>]
let ``An valid command succeeds``(command : string, switchCount : int) = 
    let listener = DummyListener()
    let controller = new Controller(switchCount, listener, MockWaveIn())
    let expected = true
    let actual =
        try 
            listener.Do command
            true
        with 
        | _ -> false
    actual |> should equal expected

let CommandTestCaseSource =
   [|
      // Base case: from empty to recording:
      TestCaseData("D1", 2, 1, ToggleState.Recording)
      // Start recording then start playing back:
      TestCaseData("D1,D1", 2, 1, ToggleState.Playing)
      // Start recording on 1 then start recording on 0. 1 now mutes so we don't record on two records at the same time:
      TestCaseData("D1,D0", 2, 1, ToggleState.Muted) 
      // Check recording then playing on 0 leaves 1 unaffected:
      TestCaseData("D0,D0", 2, 1, ToggleState.Empty)
   |]

[<Test; TestCaseSource("CommandTestCaseSource")>]
let ``An valid sequence of commands changes the state of the correct recorder appropriately``(case : string * int * int * ToggleState) = 
    let commands, switchCount, recorderIndex, expected = case
    let listener = DummyListener()
    let controller = new Controller(switchCount, listener, MockWaveIn())
    for cmd in commands.Split([|','|]) do
        listener.Do cmd
    let actual = controller.Recorders.[recorderIndex].Toggler.State
    actual |> should equal expected

//open NAudio.Wave
//
//// TODO remove/improve
//[<Test>]
//let ``Basic recording test``() =
//   let dataAvailable args =
//     printfn "Data available"
//   let recordingStopped args =
//     printfn "Recording stopped"
//
//   let _waveInEvent = new WaveInEvent()
//   _waveInEvent.WaveFormat = new WaveFormat(44100, 1) |> ignore
//   _waveInEvent.BufferMilliseconds <- 100
//   _waveInEvent.NumberOfBuffers <- 2
//   _waveInEvent.DeviceNumber <- 0   
//   _waveInEvent.DataAvailable.Add(fun args -> dataAvailable args)
//   _waveInEvent.RecordingStopped.Add(fun args -> recordingStopped args)
//   _waveInEvent.StartRecording()
//   System.Threading.Thread.Sleep 1000
//   _waveInEvent.StopRecording()
//   true |> should equal true