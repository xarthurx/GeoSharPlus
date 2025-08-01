﻿## Setup
### Requirement
The cpp sub-project use `vcpkg`'s manifest mode.
Please install the `vcpkg` on your system and set the `VCPKG_ROOT` environment variable to the path of your `vcpkg` installation.

### Configuration
To install the required packages and configure the project, run the following commands in your terminal:
```pwsh
# Go into the GeoSharPlusCPP directory
cd GeoSharPlusCPP

# Install the required packages	
vcpkg install

# Cmake configure
cmake -B build .
```

After running the above commands, you should see a `generated` folder inside root folder of this project:
```
- GeoSharPlus/  
 ├── GeoSharPlusCPP/  
 ├── GeoSharPlusNET/  
 ├── GSPdemoGH/        <--- demo GH project
 ├── GSPdemoConsole/   <--- demo Console project
 ├── cppPrebuild/      <--- collective location for the cpp lib (`.dll` (Windows) or `.dylib` (MacOS))
 └── generated/        <--- generated `.h` and `.cs` for the `flatbuffers` lib
```

## Library Demo
Now you can open the `GSP.DEMO.sln` solution file in Visual Studio and build the project.

## Integration into another Project
**The most important two folders are `GeoSharPlusCPP` and `GeoSharPlusNET`.**

### Required steps:
Copy the above two mentioned folders into your own project, and do:
1. Modify the `CMakeList.txt` file in the `GeoSharPlusCPP` folder to add:
    - any `cpp` lib that you want to use;
    - any additional pre-compilation process you want to integrate;
1. Conduct the processes described in the [Setup](#setup) section.

### Additional steps:
In your Grasshopper or Rhino plugin project, you need to add the two sub-projects:
- Right-click your main project -> `Add` -> `Project Reference...` -> Select the `GeoSharPlusNET` project;
- Check the `prebuild.ps1` and `postbuild.ps1` scripts in the `GSPdemoGH` folder for additional *build event* and copy to your project.

## Used by:
This library is trusted by these notable projects:
- [BeingAliveLanguage](https://beingalivelanguage.arch.ethz.ch) - A Grasshopper plugin to create computational diagrams for living systems.
- [IG-Mesh](https://github.com/xarthurx/IG-Mesh) - An one-stop solution for mesh processing in Grasshopper (for Rhino).

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
