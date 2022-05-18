using Google.Cloud.Firestore;
using Podcast.Api.Podcast;
using System.Web;

namespace Podcast.Api.PodcastDetails
{
    public class PodcastDetailsEndpoint : Endpoint<PodcastDetailsRequest, EmptyResponse>
    {
        private readonly FirestoreDb firestoreDb;
        private readonly CollectionReference podcastIdCollection;
        private readonly CollectionReference podcastDetailsCollection;
        private const string URL = "https://itunes.apple.com/lookup?id=";

        public PodcastDetailsEndpoint(FirestoreDb firestoreDb)
        {
            this.firestoreDb = firestoreDb;
            this.podcastIdCollection = firestoreDb.Collection("Podcast-ids");
            this.podcastDetailsCollection = firestoreDb.Collection("Podcasts");
        }

        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("/api/podcastdetails");
            AllowAnonymous();
        }

        public override async Task HandleAsync(PodcastDetailsRequest req, CancellationToken ct)
        {
            // Get the Podcastids by the Podcast Name 
            var podcastids = await GetPodcastIds(req.Name);
            List<Task> tasks = new List<Task>();
            foreach (var item in PagedIterator(podcastids.PodcastIds, 200))
            {
                HttpClient httpClient = new HttpClient();
                var requestUrl = $"{URL}{string.Join<int>(",", item)}";
                var response = await httpClient.GetFromJsonAsync<PodcastDetailsResult>(requestUrl);
                foreach (var podcastDetails in response.PodcastDetails)
                {
                    DocumentReference documentReference = this.podcastDetailsCollection
                                                       .Document(podcastDetails.PodcastName.Replace("/", ":"));
                    tasks.Add(documentReference.SetAsync(podcastDetails, SetOptions.Overwrite));

                }
            }
            await Task.WhenAll(tasks);
            await SendNoContentAsync(ct);
        }

        private async Task<PodcastIdentifier> GetPodcastIds(string name)
        {
            DocumentReference documentReference = this.podcastIdCollection.Document(name);
            var snapshot = await documentReference.GetSnapshotAsync();
            return snapshot.ConvertTo<PodcastIdentifier>();
        }

        private IEnumerable<List<T>> PagedIterator<T>(IEnumerable<T> objectList, int PageSize)
        {
            var page = 0;
            var recordCount = objectList.Count();
            var pageCount = (int)((recordCount + PageSize) / PageSize);

            if (recordCount < 1)
            {
                yield break;
            }

            while (page < pageCount)
            {
                var pageData = objectList.Skip(PageSize * page).Take(PageSize).ToList();

                yield return pageData;
                page++;
            }
        }
    }
}
