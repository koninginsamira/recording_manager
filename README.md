<h1 align="center">Recording Manager</h1>
<h3 align="center">Part of the <a href="https://jellyfin.org">Jellyfin Project</a></h3>

<p align="center">
Recording Manager is a plugin that adds to the existing functionality for recording Live TV from within Jellyfin;

</p>

## Install Process


## From Repository
1. In jellyfin, go to dashboard -> plugins -> Repositories -> add and paste this link https://raw.githubusercontent.com/koninginsamira/jellyfin_repository/refs/heads/main/manifest.json
2. Go to Catalog and search for the plugin you want to install
3. Click on it and install
4. Restart Jellyfin


## From .zip file
1. Download the .zip file from release page
2. Extract it and place the .dll file in a folder called ```plugins/Recording Manager``` under  the program data directory or inside the portable install directory
3. Restart Jellyfin

## User Guide
1. To merge your movies or episodes you can do it from Schedule task or directly from the configuration of the plugin.
2. Spliting is only avaible through the configuration



## Build Process
1. Clone or download this repository
2. Ensure you have .NET Core SDK setup and installed
3. Build plugin with following command.
```sh
dotnet publish --configuration Release --output bin
```
4. Place the resulting .dll file in a folder called ```plugins/Merge versions``` under  the program data directory or inside the portable install directory

