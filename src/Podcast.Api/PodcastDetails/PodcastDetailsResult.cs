using System.Text.Json.Serialization;

namespace Podcast.Api.PodcastDetails
{
    public class PodcastDetailsResult
    {
        [JsonPropertyName("resultCount")]
        public int ResultCount { get; set; }

        [JsonPropertyName("results")]
        public List<PodcastDetails> PodcastDetails { get; set; }
    }
}
