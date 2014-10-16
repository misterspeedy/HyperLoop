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

[<Test>]
let ``When the loop provider is initalized with a buffer containing 1 byte and 0 bytes are read no bytes are retrieved and the function returns 0``() = 
   let expectedContent, expectedCount = [|0uy|], 0
   let bufferIn = [|255uy|]
   let bufferOut = Array.zeroCreate 1
   let sut = LoopProvider(bufferIn)
   let actualCount = (sut :> IWaveProvider).Read(bufferOut, 0, 0)
   let actualContent = bufferOut
   actualCount |> should equal expectedCount
   actualContent |> should equal expectedContent

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
let ``When the loop provider is initalized with a buffer containing 10 bytes and 11 bytes are read the correct bytes are retrieved including repetition and the function returns 11``() = 
   let expectedContent, expectedCount = Array.append [|0uy..9uy|] [|0uy|], 11
   let bufferIn = [|0uy..9uy|]
   let bufferOut = Array.zeroCreate 11
   let sut = LoopProvider(bufferIn)
   let actualCount = (sut :> IWaveProvider).Read(bufferOut, 0, 11)
   let actualContent = bufferOut
   actualCount |> should equal expectedCount
   actualContent |> should equal expectedContent

[<Test>]
let ``When the loop provider is initalized with a buffer containing 10 bytes and 21 bytes are read the correct bytes are retrieved including repetition and the function returns 21``() = 
   let expectedContent, expectedCount = Array.concat [| [|0uy..9uy|]; [|0uy..9uy|]; [|0uy|] |], 21
   let bufferIn = [|0uy..9uy|]
   let bufferOut = Array.zeroCreate 21
   let sut = LoopProvider(bufferIn)
   let actualCount = (sut :> IWaveProvider).Read(bufferOut, 0, 21)
   let actualContent = bufferOut
   actualCount |> should equal expectedCount
   actualContent |> should equal expectedContent

[<Test>]
let ``When the loop provider is initalized with a buffer containing 10 bytes and 5 bytes are read into position 4 the correct bytes and count are returned``() = 
   let expectedContent, expectedCount = Array.concat [| [|0uy;0uy;0uy;0uy|]; [|0uy..4uy|]; [|0uy|] |], 5
   let bufferIn = [|0uy..9uy|]
   let bufferOut = Array.zeroCreate 10
   let sut = LoopProvider(bufferIn)
   let actualCount = (sut :> IWaveProvider).Read(bufferOut, 4, 5)
   let actualContent = bufferOut
   actualCount |> should equal expectedCount
   actualContent |> should equal expectedContent

[<Test>]
let ``When the loop provider is initalized with a buffer containing 10 bytes and 20 bytes are read into position 4 the correct bytes and count are returned including repetition``() = 
   let expectedContent, expectedCount = [|0uy; 0uy; 0uy; 0uy; 0uy; 1uy; 2uy; 3uy; 4uy; 5uy; 6uy; 7uy; 8uy; 9uy; 0uy; 1uy; 2uy; 3uy; 4uy; 5uy; 6uy; 7uy; 8uy; 9uy|], 20
   let bufferIn = [|0uy..9uy|]
   let bufferOut = Array.zeroCreate 24
   let sut = LoopProvider(bufferIn)
   let actualCount = (sut :> IWaveProvider).Read(bufferOut, 4, 20)
   let actualContent = bufferOut
   actualCount |> should equal expectedCount
   actualContent |> should equal expectedContent

[<Test>]
let ``When more bytes are requested than will fit in the buffer when reading into position 0 an error is raised``() = 
   let bufferIn = [|0uy..9uy|]
   let bufferOut = Array.zeroCreate 10
   let sut = LoopProvider(bufferIn)
   (fun () -> (sut :> IWaveProvider).Read(bufferOut, 0, 11) |> ignore) |> should throw typeof<System.IndexOutOfRangeException>

[<Test>]
let ``When more bytes are requested than will fit in the buffer when reading into position > 0 an error is raised``() = 
   let bufferIn = [|0uy..9uy|]
   let bufferOut = Array.zeroCreate 10
   let sut = LoopProvider(bufferIn)
   (fun () -> (sut :> IWaveProvider).Read(bufferOut, 9, 5) |> ignore) |> should throw typeof<System.IndexOutOfRangeException>

[<Test>]
let ``When two successive reads occur the read position is advanced so the correct sequence of bytes is returned``() = 
   let expected = [|0uy..9uy|]
   let bufferIn = [|0uy..9uy|]
   let bufferOut = Array.zeroCreate 10
   let sut = LoopProvider(bufferIn)
   for pos in [|0; 5|] do
      (sut :> IWaveProvider).Read(bufferOut, pos, 5) |> ignore
   let actual = bufferOut
   actual |> should equal expected

[<Test>]
let ``When two successive reads occur and there is a wraparound the read position is cycled so the correct sequence of bytes is returned``() = 
   let expected = Array.append [|0uy..9uy|] [|0uy..1uy|]
   let bufferIn = [|0uy..9uy|]
   let bufferOut = Array.zeroCreate 12
   let sut = LoopProvider(bufferIn)
   for pos in [|0; 6|] do
      (sut :> IWaveProvider).Read(bufferOut, pos, 6) |> ignore
   let actual = bufferOut
   actual |> should equal expected