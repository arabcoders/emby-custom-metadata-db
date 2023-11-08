using System;
using System.Runtime.Serialization;
using System.Text.Json;

namespace CustomMetadataDB.Helpers
{
    /// <summary>
    /// Object that represent how data from yt-dlp should look like.
    /// </summary>
    public sealed class DTO
    {
        /// <summary>
        /// Provider Id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Id { get; set; }
        /// <summary>
        /// Series Title
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Title { get; set; }
        /// <summary>
        /// List of strings that match the search query.
        /// </summary>
        public string[] Matcher { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Series plot summary.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Actors list.
        /// </summary>
        public Actors[] Actors { get; set; } = Array.Empty<Actors>();
        /// <summary>
        /// Series Genre.
        /// </summary>
        public string[] Genres { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Studio name.
        /// </summary>
        public string Studio { get; set; }
        /// <summary>
        /// Parental Guidance Rating.
        /// </summary>
        public string RatingGuide { get; set; }
        /// <summary>
        /// Series Premiere Date.
        /// </summary>
        public DateTime? Premiere { get; set; } = null;
        /// <summary>
        /// Series rating.
        /// </summary>
        public float? Rating { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public sealed class Actors
    {
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
