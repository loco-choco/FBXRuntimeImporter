## FBXRuntimeImporter

This is a project that aims to help parsing and importing at runtime the information stored in the autodesk's file format _FBX_.
The idea is for it to work like the [ObjImporter](https://wiki.unity3d.com/index.php?title=ObjImporter&oldid=13033), but it can work  in any other enviroment.
The only reference ([Ionic.Zlib in DotNetZip](https://github.com/DinoChiesa/DotNetZip) from [DinoChiesa](https://github.com/DinoChiesa)) is embeded to the code, 
so that any person that wants to use it can just copy the files and compile it as a separated dll or place it inside their code.


### Solution specifications

The solution is set to compile in .Net Framework 2.0, which means that it can run in higher .Net Framework versions.


### What it can and cannot do?

The project, for now, can parse the file into the node format and, from that, extract animation information (it can work with multiples animation per file).
The project still cannot extract mesh data, material data, full armature data (only bone names for the animation data), video data and the other data types
embeded in a fbx file. There are some rare cases where the animation data is not read correctly, that usually happens when other things then bones are animated.


Fell free to use it in any project that you want and to help the project
