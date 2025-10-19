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
            var songs = await _playlistService.GetSongsInPlaylistAsync(playlistId);
            return songs is not null ? Ok(songs) : NotFound();
        }

        [HttpPost("{playlistId:guid}/add/{songId:guid}")]
        public async Task<IActionResult> AddSongToPlaylist(Guid playlistId, Guid songId)
        {
            var result = await _playlistService.AddSongToPlaylistAsync(playlistId, songId);
            return result.Success ? Ok(result.Songs) : NotFound(result.ErrorMessage);
        }

        [HttpDelete("{playlistId:guid}/delete/{songId:guid}")]
        public async Task<IActionResult> RemoveSongFromPlaylist(Guid playlistId, Guid songId)
        {
            var result = await _playlistService.RemoveSongFromPlaylistAsync(playlistId, songId);
            return result.Success ? Ok() : NotFound(result.ErrorMessage);
        }

        [HttpPost("{playlistId:guid}/vote/{songId:guid}")]
        public async Task<IActionResult> UpvoteSongInPlaylist(Guid playlistId, Guid songId)
        {
            var result = await _playlistService.UpvoteSongInPlaylistAsync(playlistId, songId);
            return result.Success ? Ok(result.Songs) : NotFound(result.ErrorMessage);
        }

        [HttpPost("{playlistId:guid}/play/{songId:guid}")]
        public async Task<IActionResult> GetCurrentSong(Guid playlistId, Guid songId)
        {
            var result = await _playlistService.SetCurrentSongAsync(playlistId, songId);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.CurrentSong);
        }
    }
}


