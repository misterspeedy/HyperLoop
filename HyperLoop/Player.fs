﻿namespace HyperLoop

open System
open System.IO
open NAudio.Wave

type Player(waveOut : IWavePlayer) =
   member __.Play (bytes : byte[]) =
      let stream = new MemoryStream(bytes) 
      // TODO magic numbers
      let waveFormat = new WaveFormat(44100, 1)
      let rsws = new RawSourceWaveStream(stream, waveFormat)
      waveOut.Init(rsws)
      waveOut.Play()

   // TODO remove
   member __.WaveOut = waveOut
      
      