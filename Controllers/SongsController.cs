using Microsoft.AspNetCore.Mvc;
using PSI.Services;

namespace PSI.Controllers
{
    [Route("songs")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly SongService _songService;

        public SongsController(SongService songService)
        {
            _songService = songService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSongs()
        {
            var allSongs = await _songService.GetAllSongsAsync();
            return Ok(allSongs);
        }

        [HttpGet("{songId:guid}")]
        public async Task<IActionResult> GetSongById(Guid songId)
        {
            var song = await _songService.GetSongByIdAsync(songId);
            return song is not null ? Ok(song) : NotFound();
        }

        [HttpGet("play/{songId:guid}")]
        public async Task<IActionResult> GetSongLink(Guid songId)
        {
            var song = await _songService.GetSongByIdAsync(songId);
            if (song == null)
                return NotFound("Song not found");

            var directLink = _songService.ConvertDriveLink(song.Link);
            return Ok(new { link = directLink });
        }

    }
}
