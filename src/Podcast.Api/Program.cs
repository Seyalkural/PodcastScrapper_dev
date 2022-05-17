global using FastEndpoints;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Podcast.Api.Genre;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
        .MinimumLevel.Information()
        .WriteTo.Console());

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

var url = $"http://0.0.0.0:{port}";

static string GetProjectId()
{
    GoogleCredential googleCredential = Google.Apis.Auth.OAuth2
        .GoogleCredential.GetApplicationDefault();
    if (googleCredential != null)
    {
        ICredential credential = googleCredential.UnderlyingCredential;
        ServiceAccountCredential serviceAccountCredential =
            credential as ServiceAccountCredential;
        if (serviceAccountCredential != null)
        {
            return serviceAccountCredential.ProjectId;
        }
    }
    return Google.Api.Gax.Platform.Instance().ProjectId;
}

var projectId = GetProjectId();

builder.Services.AddFastEndpoints();

builder.Services.AddSingleton<FirestoreDb>(FirestoreDb.Create(projectId));
builder.Services.AddScoped<IScrapGenre,ScrapGenre>();
builder.Services.AddScoped<IGenreRepository,GenreRepository>();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseFastEndpoints();

app.Run(url);
