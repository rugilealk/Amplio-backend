using PSI.Models;
using PSI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<SongService>();
builder.Services.AddSingleton<PlaylistService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ----- Songs -----
var songs = app.MapGroup("/songs");
songs.MapGet("/", (SongService svc) => svc.GetAll());
songs.MapGet("/{songId}", (Guid songId, SongService svc) =>
{
    var s = svc.GetById(songId);
    return s is not null ? Results.Ok(s) : Results.NotFound();
});

// ----- Playlists -----
var playlists = app.MapGroup("/playlist");

playlists.MapPost("/", (PlaylistService svc) =>
{
    var pl = svc.Create();
    return Results.Created($"/playlist/{pl.PlaylistId}", new { pl.PlaylistId });
});

playlists.MapGet("/{playlistId}", (Guid playlistId, PlaylistService svc) =>
{
    var pl = svc.GetById(playlistId);
    return pl is not null ? Results.Ok(pl.GetAllSongs()) : Results.NotFound();
});

playlists.MapPost("/{playlistId}/add/{songId}",
    (Guid playlistId, Guid songId, PlaylistService pSvc, SongService sSvc) =>
{
    var pl = pSvc.GetById(playlistId);
    if (pl == null) return Results.NotFound("Playlist not found");

    var song = sSvc.GetById(songId);
    if (song == null) return Results.NotFound("Song not found");

    pl.AddSong(song);
    return Results.Ok(pl.GetAllSongs());
});

playlists.MapDelete("/{playlistId}/delete/{songId}",
    (Guid playlistId, Guid songId, PlaylistService svc) =>
{
    var pl = svc.GetById(playlistId);
    if (pl == null) return Results.NotFound();

    return pl.DeleteSong(songId) ? Results.Ok() : Results.NotFound();
});

playlists.MapPost("/{playlistId}/vote/{songId}",
    (Guid playlistId, Guid songId, PlaylistService svc) =>
{
    var pl = svc.GetById(playlistId);
    if (pl == null) return Results.NotFound();

    pl.UpvoteSong(songId);
    return Results.Ok(pl.GetAllSongs());
});

app.Run();
