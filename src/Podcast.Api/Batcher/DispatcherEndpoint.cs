using Google.Cloud.Firestore;
using Google.Cloud.Tasks.V2;
using Google.Protobuf;
using Podcast.Api.Podcast;
using System.Text.Json;

namespace Podcast.Api.Batcher
{
    public class DispatcherEndpoint : Endpoint<EmptyRequest, EmptyResponse>
    {
        private readonly FirestoreDb firestoreDb;
        private readonly ILogger<DispatcherEndpoint> logger;
        private readonly QueueName queueName;
        private readonly CloudTasksClient cloudTasksClient;
        private readonly CollectionReference collectionReference;

        public DispatcherEndpoint(FirestoreDb firestoreDb,
            ILogger<DispatcherEndpoint> logger,
            QueueName queueName,
            CloudTasksClient cloudTasksClient)
        {
            this.firestoreDb = firestoreDb;
            this.logger = logger;
            this.queueName = queueName;
            this.cloudTasksClient = cloudTasksClient;
            this.collectionReference = this.firestoreDb.Collection("genre");
        }

        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("api/dispatch");
            AllowAnonymous();
        }

        public override async System.Threading.Tasks.Task HandleAsync(EmptyRequest req, CancellationToken ct)
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
            this.logger.LogInformation("Base URL : {BaseURL}", BaseURL.Replace("http","https"));
            Dispatch(genreList);

            await SendNoContentAsync(ct);
        }

        private void Dispatch(List<Genre.Genre> genreList)
        {
            foreach (var item in genreList)
            {
                var payload = JsonSerializer.Serialize(new PodcastGenre
                {
                    Name = item.Name,
                    Url = item.Url,
                    Id = item.Id,
                });
                var httpsurl = BaseURL.Replace("http", "https");
                var request = new Google.Cloud.Tasks.V2.HttpRequest
                {
                    Url = $"{httpsurl}/api/podcast",
                    Body = ByteString.CopyFromUtf8(payload),
                    HttpMethod = Google.Cloud.Tasks.V2.HttpMethod.Post
                };
                request.Headers.Add("Content-Type", "application/json");
                var cloudTask = this.cloudTasksClient.CreateTask(new CreateTaskRequest
                {
                    Parent = this.queueName.ToString(),
                    Task = new Google.Cloud.Tasks.V2.Task
                    {
                        HttpRequest = request,
                        ScheduleTime = Google.Protobuf.WellKnownTypes
                                            .Timestamp
                                            .FromDateTime(DateTime.UtcNow.AddSeconds(5))
                    }
                });
            }
        }
    }
}
