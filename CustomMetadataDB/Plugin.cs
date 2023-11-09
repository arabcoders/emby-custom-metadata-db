using System;
using System.Collections.Generic;
using CustomMetadataDB.Helpers;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;

namespace CustomMetadataDB
{
    public class Plugin : BasePlugin, IHasWebPages
    {
        public override string Name => Constants.PLUGIN_NAME;
        public override string Description => Constants.PLUGIN_DESCRIPTION;
        public override Guid Id => Guid.Parse(Constants.PLUGIN_GUID);

        public IEnumerable<PluginPageInfo> GetPages()
        {
            var prefix = GetType().Namespace;

            return new[] {
                new PluginPageInfo
                {
                    Name = Name,
                    EmbeddedResourcePath = prefix + ".Configuration.Web.custom_metadata_db.html",
                },
                new PluginPageInfo
                {
                    Name = $"{Name}js",
                    EmbeddedResourcePath = prefix + ".Configuration.Web.custom_metadata_db.js"
                },
            };
        }
    }
}
