using Google.Cloud.Firestore;

namespace Podcast.Api.Genre
{
    public class GenreRepository : IGenreRepository
    {
        private FirestoreDb db;
        private CollectionReference collection;

        public GenreRepository(FirestoreDb firestoreDb)
        {
            this.db = firestoreDb;
            this.collection = db.Collection("genre");
        }

        public async Task SaveGenre(IEnumerable<Genre> genres)
        {
            foreach (var genre in genres)
            {
                DocumentReference documentReference = this.collection.Document(genre.Name);
                var snapshot = await documentReference.GetSnapshotAsync();
                if (!snapshot.Exists)
                    await documentReference.CreateAsync(genre);
            }
        }
    }
}
