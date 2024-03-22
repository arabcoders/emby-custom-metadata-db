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

namespace CustomMetadataDB;

public class EpisodeProvider : ILocalMetadataProvider<Episode>, IHasItemChangeMonitor
{
    protected readonly ILogger _logger;

    public string Name => Constants.PLUGIN_NAME;

    public EpisodeProvider(ILogger logger)
    {
        _logger = logger;
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
        _logger.Debug($"CMD HasChanged: Checking {item.Path}");

        FileSystemMetadata fileInfo = directoryService.GetFile(item.Path);
        bool result = fileInfo.Exists && fileInfo.LastWriteTimeUtc.ToUniversalTime() > item.DateLastSaved.ToUniversalTime();
        string status = result ? "Has Changed" : "Has Not Changed";

        _logger.Debug($"CMD HasChanged Result: {status}");

        return result;
    }
}
