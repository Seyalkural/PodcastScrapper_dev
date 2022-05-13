namespace Podcast.Api.Genre;

public class GenreEndpoint : EndpointWithoutRequest<List<Genre>>
{

    private readonly IGenreRepository genreRepository;
    private readonly IScrapGenre scrapGenre;

    public GenreEndpoint(IGenreRepository genreRepository, IScrapGenre scrapGenre)
    {
        this.genreRepository = genreRepository;
        this.scrapGenre = scrapGenre;
    }

    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("api/genres");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var genres = this.scrapGenre.GetAllGenres();
        await this.genreRepository.SaveGenre(genres);
        await SendOkAsync(genres.ToList(),ct);
    }


}