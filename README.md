# Glitch
A rewrite of [MEMZ](https://github.com/Leurak/MEMZ) in C#

# Features
- All original graphical payloads are implemented
- Includes an original payload (Remove ExtraPayloads.cs if you don't want that)
- Written in C#. This does make it slower but allows for more modularity

# Missing features
- The MBR payload would have to be copied from the original as C# doesn't compile to native code
- Currently only supports a console-based interface. Forms are coming.
- notepad is not ran on startup as it doesn't currently _destroy_ your pc