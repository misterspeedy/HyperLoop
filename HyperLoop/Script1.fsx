#r @"C:\Code\VS2013\ArduinoRead\CommandMapper\bin\Debug\CommandMapper.dll"
#r @"C:\Code\VS2013\ArduinoRead\packages\NAudio.1.7.1\lib\net35\NAudio.dll"
#r @"C:\Code\VS2013\ArduinoRead\Sampler\bin\Debug\Sampler.dll"
#r @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.dll"
#r @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Core.dll"
#r @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Numerics.dll"

open NAudio.Wave

let waveInDevices = WaveIn.DeviceCount
printfn "waveInDevices %i" waveInDevices

let mutable waveInDevice = 0

while waveInDevice < waveInDevices do
   let deviceInfo = WaveIn.GetCapabilities(waveInDevice)
   printf "Device %i %s %i channels" waveInDevice deviceInfo.ProductName deviceInfo.Channels
   waveInDevice <- waveInDevice + 1

//for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
//{
//    WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
//    Console.WriteLine("Device {0}: {1}, {2} channels", waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);
//}
