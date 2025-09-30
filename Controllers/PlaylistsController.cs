using Microsoft.AspNetCore.Mvc;
using PSI.Services;

[Route("playlist")]
[ApiController]
public class PlaylistsController : ControllerBase
{
    private readonly PlaylistService playlistService;
    private readonly SongService songService;

    public PlaylistsController(PlaylistService playlistService, SongService songService)
    {
        this.playlistService = playlistService;
        this.songService = songService;
    }

    [HttpPost]
    public IActionResult Create()
    {
        var newPlaylist = playlistService.Create();
        return Created($"/playlist/{newPlaylist.PlaylistId}", new { newPlaylist.PlaylistId });
    }

    [HttpGet("{playlistId:guid}")]
    public IActionResult GetAllSongs(Guid playlistId)
    {
        var existingPlaylist = playlistService.GetById(playlistId);
        return existingPlaylist is not null ? Ok(existingPlaylist.GetAllSongs()) : NotFound();
    }

    [HttpPost("{playlistId:guid}/add/{songId:guid}")]
    public IActionResult AddSong(Guid playlistId, Guid songId)
    {
        var existingPlaylist = playlistService.GetById(playlistId);
        if (existingPlaylist == null) return NotFound("Playlist not found");

        var song = songService.GetById(songId);
        if (song == null) return NotFound("Song not found");

        existingPlaylist.AddSong(song);
        return Ok(existingPlaylist.GetAllSongs());
    }

    [HttpDelete("{playlistId:guid}/delete/{songId:guid}")]
    public IActionResult DeleteSong(Guid playlistId, Guid songId)
    {
        var existingPlaylist = playlistService.GetById(playlistId);
        if (existingPlaylist == null) return NotFound();

        return existingPlaylist.DeleteSong(songId) ? Ok() : NotFound();
    }

    [HttpPost("{playlistId:guid}/vote/{songId:guid}")]
    public IActionResult VoteSong(Guid playlistId, Guid songId)
    {
        var existingPlaylist = playlistService.GetById(playlistId);
        if (existingPlaylist == null) return NotFound();

        existingPlaylist.UpvoteSong(songId);
        return Ok(existingPlaylist.GetAllSongs());
    }
}
