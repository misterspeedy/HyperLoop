[<NUnit.Framework.TestFixture>]
module LoopProviderTests

open System
open NUnit.Framework
open FsUnit
open HyperLoop
open Moq
open NAudio.Wave

[<Test>]
let ``When the loop provider is initalized with an empty buffer it raises an exception``() = 
   let bufferIn = [||]
   (fun () -> LoopProvider(bufferIn) |> ignore) |> should throw typeof<System.ArgumentException>

// TODO consider reading 0 bytes!

[<Test>]
let ``When the loop provider is initalized with a buffer containing 1 byte and 1 byte is read that same byte is retrieved and the function returns 1``() = 
   let expectedContent, expectedCount = [|255uy|], 1
   let bufferIn = [|255uy|]
   let bufferOut = Array.zeroCreate 1
   let sut = LoopProvider(bufferIn)
   let actualCount = (sut :> IWaveProvider).Read(bufferOut, 0, 1)
   let actualContent = bufferOut
   actualCount |> should equal expectedCount
   actualContent |> should equal expectedContent

[<Test>]
let ``When the loop provider is initalized with a buffer containing 10 bytes and 10 bytes are read those same bytes are retrieved and the function returns 10``() = 
   let expectedContent, expectedCount = [|0uy..9uy|], 10
   let bufferIn = [|0uy..9uy|]
   let bufferOut = Array.zeroCreate 10
   let sut = LoopProvider(bufferIn)
   let actualCount = (sut :> IWaveProvider).Read(bufferOut, 0, 10)
   let actualContent = bufferOut
   actualCount |> should equal expectedCount
   actualContent |> should equal expectedContent

[<Test>]
let ``When the loop provider is initalized with a buffer containing 10 bytes and 11 bytes are read the correct bytes are retrieved including repetition 11``() = 
   let expectedContent, expectedCount = Array.append [|0uy..9uy|] [|0uy|], 11
   let bufferIn = [|0uy..9uy|]
   let bufferOut = Array.zeroCreate 11
   let sut = LoopProvider(bufferIn)
   let actualCount = (sut :> IWaveProvider).Read(bufferOut, 0, 11)
   let actualContent = bufferOut
   actualCount |> should equal expectedCount
   actualContent |> should equal expectedContent

