using System.Collections.Generic;
using MediaBrowser.Common.Configuration;
using CustomMetadataDB.Helpers;
namespace CustomMetadataDB.Configuration;

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
