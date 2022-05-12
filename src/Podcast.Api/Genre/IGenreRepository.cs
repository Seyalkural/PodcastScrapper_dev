namespace Podcast.Api.Genre
{
    public interface IGenreRepository
    {
        Task SaveGenre(IEnumerable<Genre> genres);
    }
}