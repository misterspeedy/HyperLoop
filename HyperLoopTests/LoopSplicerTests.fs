[<NUnit.Framework.TestFixture>]
module SplicerTests

open System
open NUnit.Framework
open FsUnit
open HyperLoop

[<Test>]
let ``The loop splicer does nothing when provided with an empty buffer``() = 
   let expected = [||]
   let buffer = [||]
   let actual = LoopSplicer.Splice 0 buffer
   actual |> should equal expected

// TODO test count > length

[<Test>]
let ``When provided with an unvarying signal the loop splicer returns an a buffer shorter than the original by the overlap size``() = 
   let overlap = 3
   let buffer = Array.init 10 (fun _ -> 128uy)
   let expected = 10-overlap
   let actual = (LoopSplicer.Splice overlap buffer).Length
   actual |> should equal expected

[<Test>]
let ``When provided with an unvarying signal the loop splicer returns an unchanged buffer``() = 
   let overlap = 3
   let buffer = Array.init 10 (fun _ -> 128uy)
   let expected = Array.init (10-overlap) (fun _ -> 128uy)
   let actual = LoopSplicer.Splice overlap buffer
   actual |> should equal expected

[<Test>]
let ``When provided with an abitrary varying signal the loop splicer returns a buffer where the overlap region is a crossfade from the end to the start of the sample``() = 
   let overlap = 8
   let buffer = Array.init 100 (fun i -> i |> byte)
   let unchanged = buffer.[overlap..buffer.Length-overlap-1]
   let startOfBuffer, endOfBuffer = (buffer.[..overlap-1]), (buffer.[buffer.Length-overlap..])
   let splice = CrossFader.CrossFade overlap endOfBuffer startOfBuffer
   let expected = Array.append unchanged splice
   let actual = LoopSplicer.Splice overlap buffer
   actual |> should equal expected