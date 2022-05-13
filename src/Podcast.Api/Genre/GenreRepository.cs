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
            AppleUniqueIds appleUniqueIds = new AppleUniqueIds();
            DocumentReference reference = this.collection.Document("Ids");
            var snapshot = await reference.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                AppleUniqueIds appleUnique = new AppleUniqueIds();
                appleUnique.Ids = genres.Select(x => x.Id).ToList();
                _ = await reference.CreateAsync(appleUnique);
            }
            else
                appleUniqueIds = snapshot.ConvertTo<AppleUniqueIds>();
           
            foreach (var genre in genres)
            {
                if (appleUniqueIds.Ids != null && appleUniqueIds.Ids.Count != 0)
                {
                    if (!appleUniqueIds.Ids.Contains(genre.Id))
                    {
                        await this.collection.Document(genre.Name).CreateAsync(genre);
                    }
                }
                else
                {
                    DocumentReference documentReference = this.collection.Document(genre.Name);
                    var documentSnapshot = await documentReference.GetSnapshotAsync();
                    if (!documentSnapshot.Exists)
                        await documentReference.CreateAsync(genre);
                }
            }
        }
    }
}
