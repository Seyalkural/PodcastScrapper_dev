global using FastEndpoints;
using Google.Cloud.Firestore;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

var url = $"http://0.0.0.0:{port}";

builder.Services.AddFastEndpoints();

builder.Services.AddSingleton<FirestoreDb>(FirestoreDb.Create("itunes-dev"));


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseFastEndpoints();

app.Run(url);
