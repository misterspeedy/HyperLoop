namespace HyperLoop

type ToggleState = 
   | Empty
   | Recording
   | Playing
   | Muted

/// Progresses a state in a defined order on each call to Toggle:
/// Empty -> Recording, Recording -> Playing, Playing -> Muted,
/// Muted -> Playing
///
/// Also supports out-of order state changes:
/// Reset() forces the state to Empty
/// StopRecording() forces the state to Muted.
///
/// Calls OnToggle with each state change
type Toggler() as this =
   let mutable _state = ToggleState.Empty
   let _onToggle = new Event<_>()

   let ChangeState newState = 
     let oldState = _state
     _state <- newState
     _onToggle.Trigger(this, oldState, _state)

   member this.Toggle() =
     let newState =
       match _state with
       | Empty     -> Recording
       | Recording -> Playing
       | Playing   -> Muted
       | Muted     -> Playing
     ChangeState newState

   member this.Reset() =
      ChangeState ToggleState.Empty

   member this.StopRecording() =
      ChangeState ToggleState.Muted

   member __.State with get() = _state

   [<CLIEvent>]
   member this.OnToggle = _onToggle.Publish

