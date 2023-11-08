# Custom Metadata DB

This project is to make a custom database for my custom shows. You can clone and change what's needed to make it work for your use case.

## Build and Installing from source
1. Clone or download this repository.
2. Ensure you have .NET Core SDK setup and installed.
3. Build plugin with following command.
    ```
    dotnet publish --configuration CustomMetadataDB --output bin
    ```
4. Copy `CustomMetadataDB.dll` from the `bin` directory to emby plugins directory.
5. Restart emby
6. If performed correctly you will see a plugin named YTINFOReader in `Dashboard -> Plugins`.

