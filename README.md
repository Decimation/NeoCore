# NeoCore

![Icon](https://github.com/Decimation/NeoCore/raw/master/icon64.png)

Low-level utilities and tools for working with the CLR, CLR internal structures, and memory. Improved .NET Core version 
of [RazorSharp](https://github.com/Decimation/RazorSharp).

# Goals

`NeoCore` aims to provide functionality similar to that of `ClrMD`, `WinDbg SOS`, and `Reflection` but in a more detailed fashion while also exposing more underlying metadata and CLR functionality.

`NeoCore` also allows for manipulation of the CLR and low-level operations with managed objects. Additionally, `NeoCore` doesn't require attachment of a debugger to the process to acquire metadata. All metadata is acquired through memory or low-level functions.

# Features

* Calculating heap size of managed objects
* Taking the address of managed objects
* Pointer to managed types
* Pinning unblittable objects
* And much more

# Compatibility
* 64-bit (and partial 32-bit support)
* Windows
* .NET CLR
* .NET Core 3.0
* Workstation GC

# License

Icons made by <a href="https://www.freepik.com/" title="Freepik">Freepik</a>
