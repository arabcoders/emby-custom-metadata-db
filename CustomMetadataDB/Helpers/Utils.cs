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
using System.Text.RegularExpressions;

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
            return new()
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

            return new() { HasMetadata = true, Item = item };
        }

        public static EpisodeInfo FileToInfo(string file, DateTime? file_date = null)
        {

            //-- get only the file stem
            string filename = System.IO.Path.GetFileNameWithoutExtension(file);

            Match matcher = null;

            for (int i = 0; i < Constants.EPISODE_MATCHERS.Length; i++)
            {
                matcher = Constants.EPISODE_MATCHERS[i].Match(filename);
                if (!matcher.Success)
                {
                    continue;
                }
                break;
            }

            if (!matcher.Success)
            {
                Logger?.Info($"No match found for {file}.");
                return new EpisodeInfo();
            }

            string series = matcher.Groups["series"].Success ? matcher.Groups["series"].Value : "";
            string year = matcher.Groups["year"].Success ? matcher.Groups["year"].Value : "";
            year = (year.Length == 2) ? "20" + year : year;
            string month = matcher.Groups["month"].Success ? matcher.Groups["month"].Value : "";
            string day = matcher.Groups["day"].Success ? matcher.Groups["day"].Value : "";
            string episode = matcher.Groups["episode"].Success ? matcher.Groups["episode"].Value : "";

            string season = matcher.Groups["season"].Success ? matcher.Groups["season"].Value : "";
            season = season == "" ? year : season;

            string broadcastDate = (year != "" && month != "" && day != "") ? year + "-" + month + "-" + day : "";
            if (broadcastDate == "" && file_date != null)
            {
                broadcastDate = file_date?.ToString("yyyy-MM-dd") ?? "";
            }

            string title = matcher.Groups["title"].Success ? matcher.Groups["title"].Value : "";
            if (title != "")
            {
                if (!string.IsNullOrEmpty(series) && title != series && title.ToLower().Contains(series.ToLower()))
                {
                    title = title.Replace(series, "", StringComparison.OrdinalIgnoreCase).Trim();
                }

                if (title == "" && title == series && broadcastDate != "")
                {
                    title = broadcastDate;
                }

                // -- replace double spaces with single space
                title = Regex.Replace(title, @"\[.+?\]", " ").Trim('-').Trim();
                title = Regex.Replace(title, @"\s+", " ");
                title = title.Trim().Trim('-').Trim();

                if (matcher.Groups["epNumber"].Success)
                {
                    title = matcher.Groups["epNumber"].Value + " - " + title;
                }
                else if (title != "" && broadcastDate != "" && broadcastDate != title)
                {
                    title = $"{broadcastDate.Replace("-", "")} ~ {title}";
                }
            }

            if (episode == "")
            {
                episode = "1" + month + day;

                // get the modified date of the file
                if (System.IO.File.Exists(file) || file_date != null)
                {
                    episode += (file_date ?? System.IO.File.GetCreationTimeUtc(file)).ToString("mmss");
                }
            }

            episode = (episode == "") ? int.Parse('1' + month + day).ToString() : episode;

            EpisodeInfo item = new()
            {
                IndexNumber = int.Parse(episode),
                Name = title,
                Year = int.Parse(year),
                ParentIndexNumber = int.Parse(season)
            };

            item.SetProviderId(Constants.PLUGIN_EXTERNAL_ID, item.IndexNumber.ToString());

            // -- Set the PremiereDate if we have a year, month and day
            if (year != "" && month != "" && day != "")
            {
                item.PremiereDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
            }

            if (!item.IndexNumber.HasValue || item.IndexNumber.GetValueOrDefault() == 0)
            {
                Logger?.Error($"No episode number found for '{file}'.");
                return new EpisodeInfo();
            }

            Logger?.Info($"Parsed '{System.IO.Path.GetFileName(file)}' as 'S{item.ParentIndexNumber}E{item.IndexNumber}' - {item.Name}'.");

            return item;
        }

        public static MetadataResult<Episode> ToEpisode(EpisodeInfo data)
        {
            if (data.Name == "")
            {
                Logger?.Warn($"No metadata found for '{data}'.");
                return ErrorOutEpisode();
            }

            Logger?.Debug($"Processing {data}.");

            Episode item = new()
            {
                Name = data.Name,
                IndexNumber = data.IndexNumber,
                ParentIndexNumber = data.ParentIndexNumber,
            };

            if (data.PremiereDate is DateTimeOffset time)
            {
                item.PremiereDate = time;
                item.ProductionYear = time.Year;
                try
                {
                    item.SortName = $"{time.Year}{time.Month:D2}{time.Day:D2}-{item.IndexNumber:D2} - {item.Name}";
                }
                catch { }
            }

            item.SetProviderId(Constants.PLUGIN_EXTERNAL_ID, data.ProviderIds[Constants.PLUGIN_EXTERNAL_ID]);

            return new() { HasMetadata = true, Item = item };
        }

        private static MetadataResult<Series> ErrorOut() => new() { HasMetadata = false, Item = new Series() };
        private static MetadataResult<Episode> ErrorOutEpisode() => new() { HasMetadata = false, Item = new Episode() };
    }
}
