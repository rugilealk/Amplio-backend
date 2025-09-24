using Microsoft.AspNetCore.Mvc;
using PSI.Services;

[Route("playlist")]
[ApiController]
public class PlaylistsController : ControllerBase
{
    private readonly PlaylistService _playlistService;
    private readonly SongService _songService;

    public PlaylistsController(PlaylistService playlistService, SongService songService)
    {
        _playlistService = playlistService;
        _songService = songService;
    }

    [HttpPost]
    public IActionResult Create()
    {
        var pl = _playlistService.Create();
        return Created($"/playlist/{pl.PlaylistId}", new { pl.PlaylistId });
    }

    [HttpGet("{playlistId:guid}")]
    public IActionResult GetAllSongs(Guid playlistId)
    {
        var pl = _playlistService.GetById(playlistId);
        return pl is not null ? Ok(pl.GetAllSongs()) : NotFound();
    }

    [HttpPost("{playlistId:guid}/add/{songId:guid}")]
    public IActionResult AddSong(Guid playlistId, Guid songId)
    {
        var pl = _playlistService.GetById(playlistId);
        if (pl == null) return NotFound("Playlist not found");

        var song = _songService.GetById(songId);
        if (song == null) return NotFound("Song not found");

        pl.AddSong(song);
        return Ok(pl.GetAllSongs());
    }

    [HttpDelete("{playlistId:guid}/delete/{songId:guid}")]
    public IActionResult DeleteSong(Guid playlistId, Guid songId)
    {
        var pl = _playlistService.GetById(playlistId);
        if (pl == null) return NotFound();

        return pl.DeleteSong(songId) ? Ok() : NotFound();
    }

    [HttpPost("{playlistId:guid}/vote/{songId:guid}")]
    public IActionResult VoteSong(Guid playlistId, Guid songId)
    {
        var pl = _playlistService.GetById(playlistId);
        if (pl == null) return NotFound();

        pl.UpvoteSong(songId);
        return Ok(pl.GetAllSongs());
    }
}
