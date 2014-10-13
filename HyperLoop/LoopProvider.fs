namespace HyperLoop

open System
open System.IO
open NAudio.Wave

type LoopProvider(bufferIn : byte[]) =
   // TODO magic numbers
   do
      if bufferIn.Length = 0 then
         raise (new ArgumentException("Cannot loop an empty buffer"))
   let _waveFormat = new WaveFormat(44100, 1)
   interface IWaveProvider with
      member __.Read(bufferOut : byte[], offset : int, count : int) =
         let len = bufferIn.Length
         for i in 0..count-1 do
            let index = i % len
            bufferOut.[i] <- bufferIn.[index]
         count
      member __.WaveFormat = _waveFormat