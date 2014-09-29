namespace HyperLoop

open System
open NAudio.Wave

/// Coordinates a set of switches, a serial port listener and an NAudio IWaveIn,
/// sending the commands from the serial port listener to the IWaveIn.
type Controller(switchCount : int, listener : IListener, waveIn : IWaveIn) as this =
   let _recorders = Array.init switchCount (fun _ -> Recorder(waveIn))
   do
      listener.OnCommand.Add(fun args -> this.DoCommand args.Command)

   // Make sure no two recorders are recording at the same time:
   let StopOtherRecorders exceptIndex =
      [|0..switchCount-1|] 
      |> Array.filter (fun i -> 
                        (i <> exceptIndex) 
                        && (_recorders.[i].Toggler.State = ToggleState.Recording))
      |> Array.iter (fun i -> _recorders.[i].Toggler.StopRecording())

   member private this.DoCommand cmd =
      // TODO Should this logic be moved to a parser?
      if cmd.Length <> 2 then
         raise (new ArgumentException("Invalid command"))

      let verb, switch = cmd.[0], cmd.[1]
      let ok, switchNo = Int32.TryParse (switch.ToString())

      if not ok || switchNo < 0 || switchNo >= switchCount then
         raise (new ArgumentException("Invalid switch number"))

      match verb with
      | 'D' -> _recorders.[switchNo].Toggler.Toggle()
               StopOtherRecorders switchNo
      | _ -> raise (new ArgumentException("Invalid command"))

   member this.Recorders = _recorders

