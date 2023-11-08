# Custom Metadata DB

This project is to make a custom database for my custom shows. You can clone and change what's needed to make it work for your use case.

## Build and Installing from source

1. Clone or download this repository.
1. Ensure you have .NET Core SDK setup and installed.
1. Build plugin with following command.
    ```
    dotnet publish CustomMetadataDB --configuration Release --output bin
    ```
1. Create folder named `CustomMetadataDB` in the `plugins` directory inside your Jellyfin data
   directory. You can find your directory by going to Dashboard, and noticing the Paths section.
   Mine is the root folder of the default Metadata directory.
    ```
    # mkdir <Jellyfin Data Directory>/plugins/CustomMetadataDB/
    ```
1. Place the resulting files from step 3 in the `plugins/CustomMetadataDB` folder created in step 4.
    ```
    # cp -r bin/*.dll <Jellyfin Data Directory>/plugins/CustomMetadataDB/`
    ```
1. Be sure that the plugin files are owned by your `jellyfin` user:
    ```
    # chown -R jellyfin:jellyfin /var/lib/jellyfin/plugins/CustomMetadataDB/
    ```
1. If performed correctly you will see a plugin named CustomMetadataDB in `Admin -> Dashboard -> Advanced -> Plugins`.
