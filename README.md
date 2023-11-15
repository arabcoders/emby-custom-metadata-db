# Custom Metadata DB

![Build Status](https://github.com/ArabCoders/emby-custom-metadata-db/actions/workflows/build-validation.yml/badge.svg)
![MIT License](https://img.shields.io/github/license/ArabCoders/emby-custom-metadata-db.svg)

This project is to make a custom database for my custom shows. You can clone and change what's needed to make it work for your use case.

# Server code 
It doesn't matter what server language you use, you need few rules to follow The plugin will request data from the API using GET with the following query string `?type={type}&query={folderName}`.

The API Must response with `http status code 404` for no-match. and `http status code 200` for match. The body of the `200 Response` *MUST* be in JSON format with the following structure:

```json
[
    {
        "id": "show_id",
        "title": "title",
    },...
]
```
There are more fields but which can be found in the `DTO` object. However, they are not supported at the moment.

## Build and Installing from source
1. Clone or download this repository.
2. Ensure you have .NET Core SDK setup and installed.
3. Build plugin with following command.
    ```
    dotnet publish --configuration CustomMetadataDB --output bin
    ```
4. Copy `CustomMetadataDB.dll` from the `bin` directory to emby plugins directory.
5. Restart emby
6. If performed correctly you will see a plugin named CMetadataDB in `Dashboard -> Plugins`.

