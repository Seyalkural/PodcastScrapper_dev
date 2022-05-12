using Google.Cloud.Firestore;

namespace Podcast.Api.Genre;

[FirestoreData]
public class Genre
{
    [FirestoreDocumentId]
    public string DocumentId { get; set; }

    [FirestoreProperty]
    public int Id { get; set; }

    [FirestoreProperty]
    public string Name { get; set; }

    [FirestoreProperty]
    public string Url { get; set; }
}