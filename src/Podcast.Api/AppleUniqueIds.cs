using Google.Cloud.Firestore;

namespace Podcast.Api
{
    [FirestoreData]
    public class AppleUniqueIds
    {
        [FirestoreProperty]
        public List<int> Ids { get; set; }
    }
}
