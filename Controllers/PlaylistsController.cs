using Microsoft.AspNetCore.Mvc;
using PSI.DTOs;
using PSI.Services;

namespace PSI.Controllers
{
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
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Playlist name cannot be empty.");
            }

            var createdPlaylist = await _playlistService.CreatePlaylistAsync(request.Name);
            return Created($"/playlist/{createdPlaylist.PlaylistId}", new { createdPlaylist.PlaylistId, createdPlaylist.Name });
        }

        [HttpGet("{playlistId:guid}")]
        public async Task<IActionResult> GetSongsInPlaylist(Guid playlistId)
        {
            var playlist = await _playlistService.GetPlaylistByIdAsync(playlistId);
            return playlist is not null ? Ok(playlist.GetAllSongs()) : NotFound();
        }

        [HttpPost("{playlistId:guid}/add/{songId:guid}")]
        public async Task<IActionResult> AddSongToPlaylist(Guid playlistId, Guid songId)
        {
            var playlist = await _playlistService.GetPlaylistByIdAsync(playlistId);
            if (playlist == null) return NotFound("Playlist not found");

            var song = await _songService.GetSongByIdAsync(songId);
            if (song == null) return NotFound("Song not found");

            playlist.AddSong(song);
            await _playlistService.SaveChangesAsync();
            return Ok(playlist.GetAllSongs());
        }

        [HttpDelete("{playlistId:guid}/delete/{songId:guid}")]
        public async Task<IActionResult> RemoveSongFromPlaylist(Guid playlistId, Guid songId)
        {
            var playlist = await _playlistService.GetPlaylistByIdAsync(playlistId);
            if (playlist == null) return NotFound();

            var wasDeleted = playlist.DeleteSong(songId);
            if (!wasDeleted) return NotFound();

            await _playlistService.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{playlistId:guid}/vote/{songId:guid}")]
        public async Task<IActionResult> UpvoteSongInPlaylist(Guid playlistId, Guid songId)
        {
            var playlist = await _playlistService.GetPlaylistByIdAsync(playlistId);
            if (playlist == null) return NotFound();

            playlist.UpvoteSong(songId);
            await _playlistService.SaveChangesAsync();
            return Ok(playlist.GetAllSongs());
        }
    }
}


