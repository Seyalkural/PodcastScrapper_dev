using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace Podcast.Api.Batcher
{
    public class DispatcherEndpoint : Endpoint<EmptyRequest, EmptyResponse>
    {
        private readonly FirestoreDb firestoreDb;
        private readonly ILogger<DispatcherEndpoint> logger;
        private readonly CollectionReference collectionReference;

        public DispatcherEndpoint(FirestoreDb firestoreDb, ILogger<DispatcherEndpoint> logger)
        {
            this.firestoreDb = firestoreDb;
            this.logger = logger;
            this.collectionReference = this.firestoreDb.Collection("genre");
        }

        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("api/dispatch");
            AllowAnonymous();
        }

        public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
        {
            List<Genre.Genre> genreList = new List<Genre.Genre>();
            await foreach (var item in this.collectionReference.ListDocumentsAsync())
            {
                var document = await item.GetSnapshotAsync();
                var genre = document.ConvertTo<Genre.Genre>();
                genreList.Add(genre);
            }
            var count = genreList.Count;
            this.logger.LogInformation("Fetched {count} of Genre", count);
            this.logger.LogInformation("Base URL : {BaseURL}", BaseURL);
            await SendNoContentAsync(ct);
        }
    }
}
