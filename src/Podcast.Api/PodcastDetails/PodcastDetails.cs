using Google.Cloud.Firestore;
using System.Text.Json.Serialization;

namespace Podcast.Api.PodcastDetails
{
    [FirestoreData]
    public class PodcastDetails
    {
        [FirestoreProperty]
        [JsonPropertyName("collectionName")]
        public string PodcastName { get; set; }

        [FirestoreProperty]
        [JsonPropertyName("feedUrl")]
        public string FeedUrl { get; set; }

        [FirestoreProperty]
        [JsonPropertyName("artworkUrl30")]
        public string SmallImageURL { get; set; }

        [FirestoreProperty]
        [JsonPropertyName("artworkUrl60")]
        public string MediumImageURL { get; set; }

        [FirestoreProperty]
        [JsonPropertyName("artworkUrl100")]
        public string LargeImageUrl { get; set; }

        [FirestoreProperty]
        [JsonPropertyName("artworkUrl600")]
        public string ExtralargeImageUrl { get; set; }

        [FirestoreProperty]
        [JsonPropertyName("primaryGenreName")]
        public string PrimaryGenre { get; set; }

        [FirestoreProperty]
        [JsonPropertyName("genres")]
        public List<string> Genres { get; set; }
    }
}
