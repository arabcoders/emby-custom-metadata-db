using CustomMetadataDB.Helpers;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Providers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediaBrowser.Model.Logging;
using MediaBrowser.Common.Net;
using System.Net;
using System.Text.Json;
using MediaBrowser.Model.Entities;
using System.Net.Http;

namespace CustomMetadataDB;

public class SeriesProvider : IRemoteMetadataProvider<Series, SeriesInfo>
{
    protected readonly IServerConfigurationManager _config;
    protected readonly IHttpClient _httpClient;
    protected readonly ILogger _logger;

    public SeriesProvider(IServerConfigurationManager config, IHttpClient httpClient, ILogger logger)
    {
        _config = config;
        _logger = logger;
        Utils.Logger = logger;
        _httpClient = httpClient;
    }

    public string Name => Constants.PLUGIN_NAME;

    public virtual Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken)
    {
        _logger.Debug($"CMD GetMetadata: {info.Name}");

        return GetMetadataImpl(info, cancellationToken);
    }

    internal async Task<MetadataResult<Series>> GetMetadataImpl(SeriesInfo info, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        MetadataResult<Series> result = new();
        _logger.Debug($"CMD Series GetMetadata: {info.Name}");

        try
        {
            using var httpResponse = await QueryAPI("series", info.Name, cancellationToken).ConfigureAwait(false);

            if (HttpStatusCode.OK != httpResponse.StatusCode)
            {
                _logger.Info($"CMD Series GetMetadata: {info.Name} - Status Code: {httpResponse.StatusCode}");
                return result;
            }

            DTO[] seriesRootObject = await JsonSerializer.DeserializeAsync<DTO[]>(
                utf8Json: httpResponse.Content,
                options: Utils.JSON_OPTS,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            _logger.Debug($"CMD Series GetMetadata Result: {seriesRootObject}");
            return Utils.ToSeries(seriesRootObject[0]);
        }
        catch (HttpRequestException exception)
        {
            if (exception.StatusCode.HasValue && exception.StatusCode.Value == HttpStatusCode.NotFound)
            {
                return result;
            }
            throw;
        }
    }

    protected Task<HttpResponseInfo> QueryAPI(string type, string name, CancellationToken cancellationToken, int limit = 1)
    {
        var apiUrl = Utils.GetConfiguration(_config).ApiUrl;

        apiUrl += string.IsNullOrEmpty(new Uri(apiUrl).Query) ? "?" : "&";
        apiUrl += $"type={type}&limit={limit}&&query={HttpUtility.UrlEncode(name)}";

        return _httpClient.SendAsync(new MediaBrowser.Common.Net.HttpRequestOptions
        {
            Url = apiUrl,
            BufferContent = false,
            CancellationToken = cancellationToken,
        }, "GET");
    }

    public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeriesInfo searchInfo, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.Debug($"CMD Series GetMetadata: {searchInfo.Name}");

        var result = new List<RemoteSearchResult>();

        try
        {
            using var httpResponse = await QueryAPI("series", searchInfo.Name, cancellationToken, limit: 20).ConfigureAwait(false);

            if (HttpStatusCode.OK != httpResponse.StatusCode)
            {
                _logger.Info($"CMD Series GetMetadata: {searchInfo.Name} - Status Code: {httpResponse.StatusCode}");
                return result;
            }

            DTO[] seriesRootObject = await JsonSerializer.DeserializeAsync<DTO[]>(
                             utf8Json: httpResponse.Content,
                             options: Utils.JSON_OPTS,
                             cancellationToken: cancellationToken
                         ).ConfigureAwait(false);


            foreach (var series in seriesRootObject)
            {
                result.Add(new RemoteSearchResult
                {
                    Name = series.Title,
                    ProviderIds = new ProviderIdDictionary(new Dictionary<string, string> { { Constants.PLUGIN_EXTERNAL_ID, series.Id } }),
                });
            }

            _logger.Debug($"CMD Series GetMetadata Result: {result}");
            return result;
        }
        catch (HttpRequestException exception)
        {
            if (exception.StatusCode.HasValue && HttpStatusCode.NotFound == exception.StatusCode.Value)
            {
                return result;
            }

            throw;
        }
    }
    public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken) => throw new NotImplementedException();
}
