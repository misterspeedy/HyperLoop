namespace HyperLoop

open System
open System.IO
open NAudio.Wave

type Player(waveOut : IWavePlayer) =
   member __.Play (bytes : byte[]) =
      let loopProvider = LoopProvider(bytes)
      waveOut.Init(loopProvider)
      waveOut.Play()
   member __.WaveOut = waveOut
      
      