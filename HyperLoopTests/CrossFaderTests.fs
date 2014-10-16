[<NUnit.Framework.TestFixture>]
module CrossFaderTests

open System
open NUnit.Framework
open FsUnit
open HyperLoop

[<Test>]
let ``The cross fader returns two buffers with the same length as the originals``() = 
   let expected = 10, 10
   let buffer1, buffer2 = Array.zeroCreate 10, Array.zeroCreate 10
   let result1, result2 = CrossFader.Fade 3 buffer1 buffer2
   let actual = result1.Length, result2.Length
   Assert.AreEqual(actual, expected)

[<Test>]
let ``When the originals have the same levels the results are the same as the originals``() = 
   let buffer1 = Array.init 10 (fun i -> i |> byte)
   let buffer2 = buffer1
   let expected = buffer1, buffer2
   let actual = CrossFader.Fade 3 buffer1 buffer2
   Assert.AreEqual(actual, expected)

[<Test>]
let ``Results outside the cross fade count are unchanged``() = 
   let buffer1 = Array.init 10 (fun i -> i |> byte)
   let buffer2 = Array.init 10 (fun i -> 9-i |> byte)
   let expected = buffer1.[..6], buffer2.[3..]
   let result1, result2 = CrossFader.Fade 3 buffer1 buffer2
   let actual = result1.[..6], result2.[3..]
   Assert.AreEqual(actual, expected)