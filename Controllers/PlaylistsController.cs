using Microsoft.AspNetCore.Mvc;
using PSI.DTOs;
using PSI.Services;
using PSI.Exceptions;
using System.IO;

namespace PSI.Controllers
{
    [Route("playlist")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly PlaylistService _playlistService;

        public PlaylistsController(PlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Playlist name cannot be empty.");

            var playlist = await _playlistService.CreatePlaylistAsync(request.Name, request.IsPublic, request.CurrentSongId);
            return Created($"/playlist/{playlist.Id}", new { playlist.Id, playlist.Name, playlist.IsPublic, playlist.CurrentSongId });
        }

        [HttpGet("{playlistId:guid}")]
        public async Task<IActionResult> GetSongsInPlaylist(Guid playlistId)
        {
            try
            {
                var songs = await _playlistService.GetSongsInPlaylistAsync(playlistId);
                return Ok(songs);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{playlistId:guid}/add/{songId:guid}")]
        public async Task<IActionResult> AddSongToPlaylist(Guid playlistId, Guid songId)
        {
            try
            {
                var songs = await _playlistService.AddSongToPlaylistAsync(playlistId, songId);
                return Ok(songs);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{playlistId:guid}/delete/{songId:guid}")]
        public async Task<IActionResult> RemoveSongFromPlaylist(Guid playlistId, Guid songId)
        {
            try
            {
                await _playlistService.RemoveSongFromPlaylistAsync(playlistId, songId);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{playlistId:guid}/vote/{songId:guid}")]
        public async Task<IActionResult> UpvoteSongInPlaylist(Guid playlistId, Guid songId)
        {
            try
            {
                var songs = await _playlistService.UpvoteSongInPlaylistAsync(playlistId, songId);
                return Ok(songs);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{playlistId:guid}/play")]
        public async Task<IActionResult> SetCurrentSong(Guid playlistId)
        {
            try
            {
                var currentSong = await _playlistService.SetCurrentSongAsync(playlistId);
                return Ok(currentSong);
            }
            catch (PlaylistOperationException ex)
            {
                System.IO.File.AppendAllText("logs.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{playlistId:guid}/current")]
        public async Task<IActionResult> GetCurrentSong(Guid playlistId)
        {
            try
            {
                var currentSong = await _playlistService.GetCurrentSongAsync(playlistId);
                return Ok(currentSong);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
