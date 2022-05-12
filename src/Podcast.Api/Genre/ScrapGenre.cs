using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System.Web;

namespace Podcast.Api.Genre
{
    public class ScrapGenre : IScrapGenre
    {
        public IEnumerable<Genre> GetAllGenres()
        {
            var url = "https://itunes.apple.com/us/genre/podcasts/id26?mt=2";
            var web = new HtmlWeb();
            var doc = web.Load(url);
            IEnumerable<HtmlNode> nodes = doc.DocumentNode.QuerySelectorAll("#genre-nav a");
            var genres = nodes.Select(p => new Genre
            {
                Name = HttpUtility.HtmlDecode(p.InnerText),
                Url = p.Attributes["href"].Value,
                Id = GetIdFromString(p.Attributes["href"].Value.Split("/").Last())
            });
            return genres;
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
