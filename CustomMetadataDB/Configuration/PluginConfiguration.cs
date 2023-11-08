using System.Collections.Generic;
using MediaBrowser.Common.Configuration;

namespace CustomMetadataDB.Helpers.Configuration
{
    public class PluginConfiguration
    {
        public string ApiUrl { get; set; }
        public string ApiRefUrl { get; set; }
    }

    public class PluginConfigStore : IConfigurationFactory
    {
        public IEnumerable<ConfigurationStore> GetConfigurations()
        {
            return new ConfigurationStore[]
            {
                new() {
                    Key = Constants.PLUGIN_NAME,
                    ConfigurationType = typeof(PluginConfiguration)
                }
            };
        }
    }
}
