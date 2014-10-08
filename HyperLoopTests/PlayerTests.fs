[<NUnit.Framework.TestFixture>]
module PlayerTests 

open System
open NUnit.Framework
open FsUnit
open HyperLoop
open Moq
open NAudio.Wave

let bufferGot = ref false
let getBuffer() = 
   bufferGot := true
   [|0uy; 1uy; 2uy|]

let MockWaveOut() =
   (new Mock<IWavePlayer>()).Object

[<Test>]
let ``The player can play a buffer``() = 
   let sut = Player(MockWaveOut())
   sut.Play(getBuffer())
   let expected = true
   let actual = !bufferGot
   Assert.AreEqual(expected, actual)
