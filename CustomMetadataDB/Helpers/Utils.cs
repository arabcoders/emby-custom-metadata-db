using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Common.Configuration;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using CustomMetadataDB.Helpers.Configuration;

namespace CustomMetadataDB.Helpers
{
    public class Utils
    {
        public static readonly JsonSerializerOptions JSON_OPTS = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
        };

        public static ILogger Logger { get; set; } = null;
        public static PluginConfiguration GetConfiguration(IServerConfigurationManager config)
        {
            return config.GetConfiguration<PluginConfiguration>(Constants.PLUGIN_NAME);
        }

        public static PersonInfo CreatePerson(string name, string provider_id)
        {
            return new PersonInfo
            {
                Name = name,
                Type = PersonType.Director,
                ProviderIds = new ProviderIdDictionary(new Dictionary<string, string> { { Constants.PLUGIN_EXTERNAL_ID, provider_id } }),
            };
        }
        public static MetadataResult<Series> ToSeries(DTO data)
        {
            Logger?.Info($"Processing {data}.");

            var item = new Series();

            if (string.IsNullOrEmpty(data.Id))
            {
                Logger?.Info($"No Id found for {data}.");
                return ErrorOut();
            }

            item.SetProviderId(Constants.PLUGIN_EXTERNAL_ID, data.Id);

            if (string.IsNullOrEmpty(data.Title))
            {
                Logger?.Info($"No Title found for {data}.");
                return ErrorOut();
            }
            item.Name = data.Title;

            if (!string.IsNullOrEmpty(data.Title))
            {
                item.Overview = data.Description;
            }

            if (data.Rating != null)
            {
                item.CommunityRating = data.Rating;
            }

            if (!string.IsNullOrEmpty(data.RatingGuide))
            {
                item.OfficialRating = data.RatingGuide;
            }

            if (data.Genres.Length > 0)
            {
                item.Genres = data.Genres;
            }

            if (null != data.Premiere && data.Premiere is DateTime time)
            {
                var date = time;
                item.PremiereDate = date;
                item.ProductionYear = int.Parse(date.ToString("yyyy"));
            }

            return new MetadataResult<Series>
            {
                HasMetadata = true,
                Item = item
            };
        }
        private static MetadataResult<Series> ErrorOut()
        {
            return new MetadataResult<Series>
            {
                HasMetadata = true,
                Item = new Series()
            };
        }
    }
}
