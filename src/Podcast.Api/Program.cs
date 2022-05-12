global using FastEndpoints;
using Google.Cloud.Firestore;
using Podcast.Api.Genre;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

var url = $"http://0.0.0.0:{port}";

builder.Services.AddFastEndpoints();

builder.Services.AddSingleton<FirestoreDb>(FirestoreDb.Create("itunes-dev"));
builder.Services.AddScoped<IScrapGenre,ScrapGenre>();
builder.Services.AddScoped<IGenreRepository,GenreRepository>();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseFastEndpoints();

app.Run(url);
