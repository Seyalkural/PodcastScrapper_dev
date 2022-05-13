using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System.Web;

namespace Podcast.Api.Podcast
{
    public class PodcastEndpoint : Endpoint<PodcastGenre,EmptyResponse>
    {
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
            var genres = nodes.Select(p => new PodcastGenre
            {
                Name = HttpUtility.HtmlDecode(p.InnerText),
                Url = p.Attributes["href"].Value,
                Id = GetIdFromString(p.Attributes["href"].Value.Split("/").Last())
            });
            var podcast = genres.ToList();
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
