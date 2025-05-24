using System;
using System.Threading;
using System.Threading.Tasks;
using CustomMetadataDB.Helpers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.IO;

namespace CustomMetadataDB.Provider;

public class EpisodeProvider : ILocalMetadataProvider<Episode>, IHasItemChangeMonitor
{
    protected readonly ILogger _logger;

    public string Name => Constants.PLUGIN_NAME;

    public EpisodeProvider(ILogger logger)
    {
        _logger = logger;
        Utils.Logger = logger;
    }

    public Task<MetadataResult<Episode>> GetMetadata(ItemInfo info, LibraryOptions libraryOptions, IDirectoryService directoryService, CancellationToken cancellationToken)
    {
        var result = new MetadataResult<Episode>();

        cancellationToken.ThrowIfCancellationRequested();

        _logger.Debug($"CMD Episode GetMetadata Lookup: '{info.Name}' '({info.Path})'");

        var item = Utils.FileToInfo(info.Path);

        if (string.IsNullOrEmpty(item.Name))
        {
            _logger.Warn($"CMD Episode GetMetadata: No metadata found for '{info.Path}'.");
            return Task.FromResult(result);
        }

        return Task.FromResult(Utils.ToEpisode(item));
    }

    public bool HasChanged(BaseItem item, LibraryOptions libraryOptions, IDirectoryService directoryService)
    {
        try
        {
            FileSystemMetadata fileInfo = directoryService.GetFile(item.Path);

            if (!fileInfo.Exists)
            {
                _logger.Warn($"'{item.Path}' not found.");
                return true;
            }

            if (fileInfo.CreationTimeUtc.ToUniversalTime() > item.DateLastSaved.ToUniversalTime())
            {
                _logger.Debug($"CMD HasChanged: '{item.Path}' has changed.");
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"CMD HasChanged: For path '{item.Path}' has failed. '{ex.Message}'. {ex}");
            return true;
        }

        return false;
    }
}
