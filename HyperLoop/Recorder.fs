namespace HyperLoop

open System
open NAudio.Wave

/// Takes an NAudio IWaveIn and records, stops recording,
/// pauses, plays back etc. under the control of a Toggler
type Recorder(waveIn : IWaveIn) as this = 
   do
      // TODO magic numbers
      // TODO constructor params
      waveIn.WaveFormat = new WaveFormat(44100, 1) |> ignore
      
   // TODO Inject the toggler?
   let _toggler = Toggler()
   let _bytes = ResizeArray<byte>()

   let DataAvailable (args : WaveInEventArgs) =
      // printfn "Data available"
      // TODO make faster!
      for i in 0..args.BytesRecorded-1 do
         _bytes.Add(args.Buffer.[i])
           
   let RecordingStopped args =
      printfn "Recording stopped"
      printfn "Buffer length: %i" _bytes.Count
//      for b in _bytes do
//         printf "%A|" b

   let StopIfRecording oldState =
      if oldState = ToggleState.Recording then
         waveIn.StopRecording()

   let Reset oldState = 
      StopIfRecording oldState
      _bytes.Clear()
   let Record oldState = 
      StopIfRecording oldState
      waveIn.StartRecording()
   let Play oldState =
      // Playing doesn't mean actively doing anything - just make the record buffer
      // available
      StopIfRecording oldState
   let Mute oldState = 
      StopIfRecording oldState

   do
      _toggler.OnToggle.Add (fun (_, oldState, state) -> 
         match _toggler.State with
         | ToggleState.Empty -> Reset oldState
         | ToggleState.Recording -> Record oldState
         | ToggleState.Playing -> Play oldState
         | ToggleState.Muted -> Mute oldState
      )
      waveIn.DataAvailable.Add(fun args -> DataAvailable args)
      waveIn.RecordingStopped.Add(fun args -> RecordingStopped args)

   member __.Toggler = _toggler
   // Next - think about what is going to do the playing
   // Add unit tests for Buffer member if kept
   member __.Buffer =
      if _toggler.State <> Playing then
         raise (InvalidOperationException("Cannot access buffer if not in Playing state"))
      _bytes