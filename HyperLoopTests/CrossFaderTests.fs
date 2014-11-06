[<NUnit.Framework.TestFixture>]
module CrossFaderTests

open System
open NUnit.Framework
open FsUnit
open HyperLoop

[<Test>]
let ``The cross fader returns a buffer of length count``() = 
   let expected = 3
   let buffer1, buffer2 = Array.zeroCreate 10, Array.zeroCreate 10
   let result = CrossFader.CrossFade 3 buffer1 buffer2
   let actual = result.Length
   actual |> should equal expected

[<Test>]
let ``When the originals have the same levels the result has the same level as the originals for the splice region``() = 
   let buffer1 = Array.init 10 (fun _ -> 128uy)
   let buffer2 = buffer1
   let expected = buffer1.[7..]
   let actual = CrossFader.CrossFade 3 buffer1 buffer2
   actual |> should equal expected

[<Test>]
let ``When one original has 0 levels and the other has non-zero levels there is a linear fade from 0 to the non-zero value``() = 
   let buffer1 = Array.init 10 (fun _ -> 0uy)
   let buffer2 = Array.init 10 (fun _ -> 100uy)
   let expected = [|0; 10; 20; 30; 40; 50; 60; 70; 80; 90|] |> Array.map byte
   let actual = CrossFader.CrossFade 10 buffer1 buffer2
   actual |> should (equalWithin 10E-5) expected

[<Test>]
let ``When one original has non-zero levels and the other has 0 levels there is a linear fade from the non-zero value to 0``() = 
   let buffer1 = Array.init 10 (fun _ -> 100uy)
   let buffer2 = Array.init 10 (fun _ -> 0uy)
   let expected = [|100; 90; 80; 70; 60; 50; 40; 30; 20; 10|] |> Array.map byte
   let actual = CrossFader.CrossFade 10 buffer1 buffer2
   actual |> should (equalWithin 10E-5) expected

[<Test>]
let ``When two arbitrary signals are cross faded they are blended correctly``() = 
   let buffer1 = [|30; 10; 40; 10; 20|] |> Array.map byte
   let buffer2 = [|50; 40; 20; 30; 90|] |> Array.map byte
   let expected = [|30; 16; 32; 22; 76|] |> Array.map byte
   let actual = CrossFader.CrossFade 5 buffer1 buffer2
   actual |> should (equalWithin 10E-5) expected

