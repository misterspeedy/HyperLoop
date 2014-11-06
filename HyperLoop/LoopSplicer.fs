module LoopSplicer

let Splice overlap (buffer : byte[]) =
   let unchanged = buffer.[overlap..buffer.Length-overlap-1]
   let startOfBuffer, endOfBuffer = (buffer.[..overlap-1]), (buffer.[buffer.Length-overlap..])
   let splice = CrossFader.CrossFade overlap endOfBuffer startOfBuffer
   Array.append unchanged splice