using Google.Cloud.Firestore;

namespace Podcast.Api.Podcast
{
    [FirestoreData]
    public class PodcastIdentifier
    {
        [FirestoreProperty]
        public IEnumerable<int> PodcastIds { get; set; }
    }
}
