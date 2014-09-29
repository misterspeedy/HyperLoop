namespace HyperLoop

open NAudio.Wave

// TODO Needs its own tests

/// Takes an NAudio IWaveIn and records, stops recording,
/// pauses, plays back etc. under the control of a Toggler
type Recorder(waveInEvent : IWaveIn) as this = 
   // TODO Inject the toggler?
   let _toggler = Toggler()
   
   let dataAvailable args =
     printfn "Data available"
   let recordingStopped args =
     printfn "Recording stopped"

   do
      // TODO magic numbers
      // TODO constructor params
      waveInEvent.WaveFormat = new WaveFormat(44100, 1) |> ignore
   do
      _toggler.OnToggle.Add (fun (_, oldState, state) -> 
         match _toggler.State with
         | ToggleState.Empty -> this.Reset oldState
         | ToggleState.Recording -> this.Record oldState
         | ToggleState.Playing -> this.Play oldState
         | ToggleState.Muted -> this.Mute oldState
         | _ -> failwith "Unsupported toggle state"
      )
      waveInEvent.DataAvailable.Add(fun args -> dataAvailable args)
      waveInEvent.RecordingStopped.Add(fun args -> recordingStopped args)

   let stopIfRecording oldState =
      if oldState = ToggleState.Recording then
         waveInEvent.StopRecording()

   member __.Toggler = _toggler
   member __.Reset oldState = 
      printfn "Reset"
      stopIfRecording oldState
   member __.Record oldState = 
      printfn "Recording"
      stopIfRecording oldState

      waveInEvent.StartRecording()
   member __.Play oldState =
      printfn "Playing"
      stopIfRecording oldState
   member __.Mute oldState = 
      printfn "Muted"
      stopIfRecording oldState
