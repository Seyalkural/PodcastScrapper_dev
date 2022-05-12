using System.Web;
using Fizzler.Systems.HtmlAgilityPack;
using Google.Cloud.Firestore;
using HtmlAgilityPack;

namespace Podcast.Api.Genre;

public class GenreEndpoint : EndpointWithoutRequest<List<Genre>>
{
    private readonly FirestoreDb db;
    private readonly CollectionReference collection;

    public GenreEndpoint(FirestoreDb firestoreDb)
    {
        this.db = firestoreDb;
        this.collection = db.Collection("genre");
    }

    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("api/genres");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
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

        foreach (var genre in genres)
        {
            DocumentReference documentReference = this.collection.Document(genre.Name);
            var snapshot = await documentReference.GetSnapshotAsync();
            if (!snapshot.Exists)
                await documentReference.CreateAsync(genre);
        }
        await SendOkAsync(genres.ToList(), ct);
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