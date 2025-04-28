
## Setup

This cpp sub-project use `vcpkg`'s manifest mode.
Please install the `vcpkg` on your system and set the `VCPKG_ROOT` environment variable to the path of your `vcpkg` installation.

To install the required packages and configure the project, run the following commands in your terminal:
```pwsh
# Go into the GeoBridgeCPP directory
cd GeoBridgeCPP

# Install the required packages	
vcpkg install

# Cmake configure
cmake -B build .
```

After running the above commands, you should see a `generated` folder inside root folder of this project:
```
- GeoBridgeRHGH/  
 ├── GeoBridgeCPP/  
 ├── GeoBridgeNET/  
 ├── GeoGridgeDemoGH/  
 └── generated/
```

## Library Demo
Now you can open the `GeoBridgeDemoGH.sln` solution file in Visual Studio and build the project.

## Integration into other Projects
The most important two sub folders are `GeoBridgeCPP` and `GeoBridgeNET`.
You should copy them into your own project, and run the above steps in the #Setup section.

You can then create your Grasshopper or Rhino plugin project, or any other C# project, then do:

1. import the above two sub-projects in;
1. Right-click your main project -> `Add` -> `Project Reference...` -> Select the `GeoBridgeNET` project;


## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.