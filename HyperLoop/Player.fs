namespace HyperLoop

open System
open System.IO
open NAudio.Wave

type Player(waveOut : IWavePlayer) =
   member __.Play (bytes : byte[]) =
      let loopProvider = LoopProvider(bytes)
      waveOut.Init(loopProvider)
      waveOut.Play()
   member __.PlayLooped (bytes : byte[]) =
      let stream = new MemoryStream(bytes) 
      // TODO magic numbers
      let waveFormat = new WaveFormat(44100, 1)
      let rsws = new RawSourceWaveStream(stream, waveFormat)
      waveOut.Init(rsws)
      waveOut.PlaybackStopped.Add(fun _ -> rsws.Position <- 0L; waveOut.Play())
      waveOut.Play()
   // TODO remove
   member __.WaveOut = waveOut
      
      