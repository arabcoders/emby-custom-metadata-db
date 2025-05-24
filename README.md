# Custom Metadata DB

![Build Status](https://github.com/ArabCoders/emby-custom-metadata-db/actions/workflows/build-validation.yml/badge.svg)
![License](https://img.shields.io/github/license/ArabCoders/emby-custom-metadata-db.svg)

This is a plugin for [Emby Media Server](https://emby.media) that purely here to generate unique custom IDs for TV series. to work with
my other two plugins for [Jellyfin](https://github.com/arabcoders/jf-custom-metadata-db) and [Plex](https://github.com/arabcoders/cmdb.bundle).

**This plugin require server that respond with a unique ID for a given TV series name in the following format.**

```json5
[
    {
        "id": "jp-bded21fb4b",
        "title": "Show Real title",
        "match": [
            "Matchers"
        ]
    },
    ...
]
```

## Server implementation

For a quick server implementation please refer to [this page](https://github.com/arabcoders/cmdb.bundle?tab=readme-ov-file#server-implementation).

# Installation

Go to the Releases page and download the latest release.

Copy the `CustomMetadataDB.dll` file to the Emby plugins directory. You can find your directory by going to Dashboard, and noticing the Paths section. Mine is the root folder of the default Metadata directory.

## Building from source
1. Clone or download this repository.
2. Ensure you have .NET Core SDK setup and installed.
3. Build plugin with following command.
    ```
    dotnet publish --configuration CustomMetadataDB --output bin
    ```
4. Copy `CustomMetadataDB.dll` from the `bin` directory to emby plugins directory.
5. Restart emby.

If performed correctly you will see a plugin named CMetadataDB in `Dashboard -> Plugins`.


## Change the API URL

go to `Dashboard -> Plugins -> Custom Metadata DB -> URL for the custom database:` and change the URL to your server URL, then click on `Save`.
