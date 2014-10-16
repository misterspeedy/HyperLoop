namespace HyperLoop

open System
open System.IO
open NAudio.Wave

type LoopProvider(bufferIn : byte[]) =
   do
      if bufferIn.Length = 0 then
         raise (new ArgumentException("Cannot loop an empty buffer"))
   let mutable position = 0
   // TODO magic numbers
   let _waveFormat = new WaveFormat(44100, 1)
   interface IWaveProvider with
      // TODO separate out the wave aspect from the read aspect
      member __.Read(bufferOut : byte[], offset : int, count : int) =
         let len = bufferIn.Length
         for i in 0..count-1 do
            bufferOut.[i+offset] <- bufferIn.[position]
            position <- (position + 1) % len
         count
      member __.WaveFormat = _waveFormat