namespace HyperLoop
   
open System
open System.Collections.Generic

type CommandMapping = 
   {
      CommandString : string
      Function : unit -> unit
   }

/// Take an array of mappings from strings to functions
/// and provides a way to call a function by its string

type CommandMapper(mappings : CommandMapping[]) =
   let dict = 
      mappings
      |> Seq.map (fun m -> m.CommandString, m.Function)
      |> dict
   member __.Do(command) =
      if dict.ContainsKey command then
         dict.[command]()
      else
         raise (new ArgumentException(sprintf "Unknown command: %s" command))

