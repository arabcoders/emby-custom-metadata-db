using CustomMetadataDB.Helpers;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
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

namespace CustomMetadataDB
{
    public abstract class AbstractProvider<B, T, E> : IRemoteMetadataProvider<T, E>
        where T : BaseItem, IHasLookupInfo<E>
        where E : ItemLookupInfo, new()
    {
        protected readonly IServerConfigurationManager _config;
        protected readonly IHttpClient _httpClient;
        protected readonly ILogger _logger;
        protected readonly IFileSystem _fileSystem;

        public AbstractProvider(IServerConfigurationManager config, IHttpClient httpClient, IFileSystem fileSystem, ILogger logger)
        {
            _config = config;
            _logger = logger;
            Utils.Logger = logger;
            _fileSystem = fileSystem;
            _httpClient = httpClient;
        }

        public virtual string Name { get; } = Constants.PLUGIN_NAME;

        public virtual Task<MetadataResult<T>> GetMetadata(E info, CancellationToken cancellationToken)
        {
            _logger.Debug($"CMD GetMetadata: {info.Name}");

            return GetMetadataImpl(info, cancellationToken);
        }

        internal abstract Task<MetadataResult<T>> GetMetadataImpl(E data, CancellationToken cancellationToken);

        protected Task<HttpResponseInfo> QueryAPI(string type, string name, CancellationToken cancellationToken, int limit = 1)
        {
            var apiUrl = Utils.GetConfiguration(_config).ApiUrl;

            apiUrl += string.IsNullOrEmpty(new Uri(apiUrl).Query) ? "?" : "&";
            apiUrl += $"type={type}&limit={limit}&&query={HttpUtility.UrlEncode(name)}";

            return _httpClient.SendAsync(new HttpRequestOptions
            {
                Url = apiUrl,
                BufferContent = false,
                CancellationToken = cancellationToken,
            }, "GET");
        }
        public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(E searchInfo, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
