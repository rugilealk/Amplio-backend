using Microsoft.AspNetCore.Mvc;
using PSI.Services;
using PSI.Models;

namespace PSI.Controllers
{
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
        public async Task<IActionResult> CreatePlaylist()
        {
            var createdPlaylist = await playlistService.CreatePlaylistAsync();
            return Created($"/playlist/{createdPlaylist.PlaylistId}", new { createdPlaylist.PlaylistId });
        }

        [HttpGet("{playlistId:guid}")]
        public async Task<IActionResult> GetSongsInPlaylist(Guid playlistId)
        {
            var playlist = await playlistService.GetPlaylistByIdAsync(playlistId);
            return playlist is not null ? Ok(playlist.GetAllSongs()) : NotFound();
        }

        [HttpPost("{playlistId:guid}/add/{songId:guid}")]
        public async Task<IActionResult> AddSongToPlaylist(Guid playlistId, Guid songId)
        {
            var playlist = await playlistService.GetPlaylistByIdAsync(playlistId);
            if (playlist == null) return NotFound("Playlist not found");

            var song = await songService.GetSongByIdAsync(songId);
            if (song == null) return NotFound("Song not found");

            playlist.AddSong(song);
            await playlistService.SaveChangesAsync();
            return Ok(playlist.GetAllSongs());
        }

        [HttpDelete("{playlistId:guid}/delete/{songId:guid}")]
        public async Task<IActionResult> RemoveSongFromPlaylist(Guid playlistId, Guid songId)
        {
            var playlist = await playlistService.GetPlaylistByIdAsync(playlistId);
            if (playlist == null) return NotFound();

            var wasDeleted = playlist.DeleteSong(songId);
            if (!wasDeleted) return NotFound();

            await playlistService.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{playlistId:guid}/vote/{songId:guid}")]
        public async Task<IActionResult> UpvoteSongInPlaylist(Guid playlistId, Guid songId)
        {
            var playlist = await playlistService.GetPlaylistByIdAsync(playlistId);
            if (playlist == null) return NotFound();

            playlist.UpvoteSong(songId);
            await playlistService.SaveChangesAsync();
            return Ok(playlist.GetAllSongs());
        }
    }
}
