[<NUnit.Framework.TestFixture>]
module Tests 

open NUnit.Framework
open FsUnit
open HyperLoop

[<Test>]
let ``Toggler changes state correctly when toggled``() = 
   let toggler = Toggler()
   let expected = 
      [|
         ToggleState.Empty
         ToggleState.Recording
         ToggleState.Playing
         ToggleState.Muted
         ToggleState.Playing
         ToggleState.Muted
         ToggleState.Playing
      |]
   let actual =
      [|
         for i in 0..6 do 
            yield toggler.State
            toggler.Toggle() |> ignore
      |]
   actual |> should equal expected

[<Test>]
let ``Toggler can be reset from any state``() = 
   let expected = ToggleState.Empty
   for i in 0..9 do 
      let toggler = Toggler() 
      for j in 0..i do
         toggler.Toggle() |> ignore
         toggler.Reset()
         let actual = toggler.State
         actual |> should equal expected

[<Test>]
let ``Toggler calls OnToggle event when state is changed using Toggle``() = 
   let toggler = Toggler()
   let expected = true
   let called = ref false
   toggler.OnToggle.Add(fun _ -> called := true)
   toggler.Toggle()
   let actual = !called
   actual |> should equal expected

[<Test>]
let ``Toggler calls OnToggle event when Reset is called``() = 
   let toggler = Toggler()
   let expected = true
   let called = ref false
   toggler.OnToggle.Add(fun _ -> called := true)
   toggler.Reset()
   let actual = !called
   actual |> should equal expected

[<Test>]
let ``Toggler calls OnToggle event when StopRecording is called``() = 
   let toggler = Toggler()
   let expected = true
   let called = ref false
   // Start recording:
   toggler.Toggle()
   toggler.OnToggle.Add(fun _ -> called := true)
   toggler.StopRecording()
   let actual = !called
   actual |> should equal expected
