[<NUnit.Framework.TestFixture>]
module RecorderTests 

open System
open NUnit.Framework
open FsUnit
open HyperLoop
open Moq
open NAudio.Wave

let MockWaveIn() =
   new Mock<IWaveIn>()

[<Test>]
let ``When nothing has been recorded we cannot access the buffer``() = 
   let sut = Recorder(MockWaveIn().Object)
   (fun () -> sut.Buffer |> ignore) |> should throw typeof<InvalidOperationException>

[<Test>]
let ``When something has been recorded we can access the buffer``() = 
   let sut = Recorder(MockWaveIn().Object)
   // Start recording:
   sut.Toggler.Toggle()
   // Stop recording again:
   sut.Toggler.Toggle()
   let actual = sut.Buffer
   Assert.IsNotNull(actual)

[<Test>]
let ``When something has been recorded the buffer has the right contents``() = 
   let waveIn = MockWaveIn()
   let sut = Recorder(waveIn.Object)
   // Start recording:
   sut.Toggler.Toggle()
   // Simulate some sound data arriving:
   let datas = [| [|0uy; 1uy; 2uy|]; [|3uy; 4uy; 5uy; 6uy|] |]
   for data in datas do
      let args = new WaveInEventArgs(data, data.Length)
      waveIn.Raise((fun m -> m.DataAvailable.Add(fun _ -> ()) |> ignore), args)
   // Stop recording again:
   sut.Toggler.Toggle()
   let expected = datas |> Array.concat
   let actual = sut.Buffer
   expected |> should equal actual

