using PSI.Models;
using PSI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<PlaylistService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var playlist = app.MapGroup("/playlist");

playlist.MapGet("/", (PlaylistService svc) => svc.GetAll());

playlist.MapGet("/{id}", (Guid id, PlaylistService svc) =>
{
    var song = svc.GetById(id);
    return song is not null 
        ? Results.Ok(song) 
        : Results.NotFound();
});

playlist.MapPost("/add", (Song newSong, PlaylistService svc) =>
{
    svc.AddSong(newSong);
    return Results.Created($"/playlist/{newSong.Id}", newSong);
});

playlist.MapPost("/{id}/vote", (Guid id, PlaylistService svc) =>
{
    return svc.Upvote(id)
        ? Results.Ok(svc.GetAll())
        : Results.NotFound();
});

app.Run();
