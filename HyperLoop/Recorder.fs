namespace HyperLoop

open System
open NAudio.Wave

// TODO Needs its own tests

/// Takes an NAudio IWaveIn and records, stops recording,
/// pauses, plays back etc. under the control of a Toggler
type Recorder(waveIn : IWaveIn) as this = 
   // TODO Inject the toggler?
   let _toggler = Toggler()

   let _bytes = ResizeArray<byte>()
   
   let dataAvailable (args : WaveInEventArgs) =
      // printfn "Data available"
      // TODO make faster!
      for i in 0..args.BytesRecorded-1 do
         _bytes.Add(args.Buffer.[i])
           
   let recordingStopped args =
      printfn "Recording stopped"
      printfn "Buffer length: %i" _bytes.Count
//      for b in _bytes do
//         printf "%A|" b

   do
      // TODO magic numbers
      // TODO constructor params
      waveIn.WaveFormat = new WaveFormat(44100, 1) |> ignore
   do
      _toggler.OnToggle.Add (fun (_, oldState, state) -> 
         match _toggler.State with
         | ToggleState.Empty -> this.Reset oldState
         | ToggleState.Recording -> this.Record oldState
         | ToggleState.Playing -> this.Play oldState
         | ToggleState.Muted -> this.Mute oldState
      )
      waveIn.DataAvailable.Add(fun args -> dataAvailable args)
      waveIn.RecordingStopped.Add(fun args -> recordingStopped args)

   let stopIfRecording oldState =
      if oldState = ToggleState.Recording then
         waveIn.StopRecording()

   member __.Toggler = _toggler
   member __.Reset oldState = 
      stopIfRecording oldState
      _bytes.Clear()
   member __.Record oldState = 
      stopIfRecording oldState
      waveIn.StartRecording()
   member __.Play oldState =
      // Playing doesn't mean actively doing anything - just make the record buffer
      // available
      stopIfRecording oldState
   member __.Mute oldState = 
      stopIfRecording oldState
   // Next - think about what is going to do the playing
   // Add unit tests for Buffer member if kept
   member __.Buffer =
      if _toggler.State <> Playing then
         raise (Exception("Cannot access buffer if not in Playing state"))
      _bytes