using CustomMetadataDB.Helpers;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Logging;
using MediaBrowser.Common.Net;
using System.Text.Json;

namespace CustomMetadataDB;
public class SeasonProvider : IRemoteMetadataProvider<Season, SeasonInfo>
{
    protected readonly ILogger _logger;
    public string Name => Constants.PLUGIN_NAME;

    public SeasonProvider(ILogger logger)
    {
        _logger = logger;
        Utils.Logger = logger;
    }

    public Task<MetadataResult<Season>> GetMetadata(SeasonInfo info, CancellationToken cancellationToken)
    {
        _logger.Debug($"CMD Season GetMetadata: {JsonSerializer.Serialize(info)}");
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new MetadataResult<Season>
        {
            HasMetadata = true,
            Item = new Season
            {
                Name = info.Name,
                IndexNumber = info.IndexNumber
            }
        });
    }

    public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeasonInfo searchInfo, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken) => throw new NotImplementedException();
}