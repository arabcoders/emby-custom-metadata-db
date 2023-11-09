using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using CustomMetadataDB.Helpers;
using MediaBrowser.Controller.Configuration;

namespace CustomMetadataDB
{
    public class SeriesExternalId : IExternalId
    {
        public bool Supports(IHasProviderIds item) => item is Series;
        public string Name => Constants.PLUGIN_NAME;
        public string Key => Constants.PLUGIN_EXTERNAL_ID;
        public string UrlFormatString { get; set; }

        public SeriesExternalId(IServerConfigurationManager config)
        {
            UrlFormatString = Utils.GetConfiguration(config).ApiRefUrl;
        }
    }
}
