using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using CustomMetadataDB.Helpers;
using MediaBrowser.Controller.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using MediaBrowser.Model.Logging;
using MediaBrowser.Common.Net;

namespace CustomMetadataDB
{
    public class SeriesProvider : AbstractProvider<SeriesProvider, Series, SeriesInfo>
    {
        public SeriesProvider(IServerConfigurationManager config, IHttpClient httpClient, IFileSystem fileSystem, ILogger logger) :
        base(config, httpClient, fileSystem, logger)
        { }
        internal override async Task<MetadataResult<Series>> GetMetadataImpl(SeriesInfo info, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            MetadataResult<Series> result = new();
            _logger.Debug($"CMD Series GetMetadata: {info.Name}");

            try
            {
                using var httpResponse = await QueryAPI("series", info.Name, cancellationToken).ConfigureAwait(false);
                
                if (httpResponse.StatusCode != HttpStatusCode.OK)
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
    }
}
