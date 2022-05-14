using Fizzler.Systems.HtmlAgilityPack;
using Google.Cloud.Firestore;
using HtmlAgilityPack;
using System.Web;

namespace Podcast.Api.Podcast
{
    public class PodcastEndpoint : Endpoint<PodcastGenre,EmptyResponse>
    {
        private FirestoreDb db;
        private CollectionReference collection;

        public PodcastEndpoint(FirestoreDb firestoreDb)
        {
            this.db = firestoreDb;
            this.collection = db.Collection("Podcast-ids");
        }

        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("/api/podcast");
            AllowAnonymous();
        }

        public override async Task HandleAsync(PodcastGenre req, CancellationToken ct)
        {
            var url = req.Url;
            var web = new HtmlWeb();
            var doc = web.Load(url);
            IEnumerable<HtmlNode> nodes = doc.DocumentNode.QuerySelectorAll("#selectedcontent a");
            var podcastIds = nodes.Select(p => GetIdFromString(p.Attributes["href"].Value.Split("/").Last()));
            DocumentReference documentReference = this.collection.Document(req.Name);
            await documentReference.SetAsync(new PodcastIdentifier { PodcastIds = podcastIds }, SetOptions.Overwrite);
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
