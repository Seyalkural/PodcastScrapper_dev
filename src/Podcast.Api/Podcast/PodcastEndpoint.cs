using Fizzler.Systems.HtmlAgilityPack;
using Google.Cloud.Firestore;
using Google.Cloud.Tasks.V2;
using Google.Protobuf;
using HtmlAgilityPack;
using Podcast.Api.PodcastDetails;
using System.Text.Json;
using System.Web;

namespace Podcast.Api.Podcast
{
    public class PodcastEndpoint : Endpoint<PodcastGenre,EmptyResponse>
    {
        private FirestoreDb db;
        private readonly CloudTasksClient cloudTasksClient;
        private readonly QueueName queueName;
        private CollectionReference collection;

        public PodcastEndpoint(FirestoreDb firestoreDb, CloudTasksClient cloudTasksClient, QueueName queueName)
        {
            this.db = firestoreDb;
            this.cloudTasksClient = cloudTasksClient;
            this.queueName = queueName;
            this.collection = db.Collection("Podcast-ids");
        }

        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("/api/podcast");
            AllowAnonymous();
        }

        public override async System.Threading.Tasks.Task HandleAsync(PodcastGenre req, CancellationToken ct)
        {
            var url = req.Url;
            var web = new HtmlWeb();
            var doc = web.Load(url);
            IEnumerable<HtmlNode> nodes = doc.DocumentNode.QuerySelectorAll("#selectedcontent a");
            var podcastIds = nodes.Select(p => GetIdFromString(p.Attributes["href"].Value.Split("/").Last()));
            DocumentReference documentReference = this.collection.Document(req.Name);
            await documentReference.SetAsync(new PodcastIdentifier { PodcastIds = podcastIds }, SetOptions.Overwrite);
            var payload = JsonSerializer.Serialize(new PodcastDetailsRequest
            {
                Name = req.Name
            });
            var httpsurl = BaseURL.Replace("http", "https");
            var request = new Google.Cloud.Tasks.V2.HttpRequest
            {
                Url = $"{httpsurl}/api/podcastdetails",
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
            await SendNoContentAsync();
        }


        private int GetIdFromString(string url)
        {
            int val = 0;
            string b = string.Empty;
            for (int i = 0; i < url.Length; i++)
            {
                if (Char.IsDigit(url[i]))
                    b += url[i];
            }

            if (b.Length > 0)
                val = int.Parse(b);
            return val;
        }
    }
}
