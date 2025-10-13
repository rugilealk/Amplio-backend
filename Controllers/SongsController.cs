using Microsoft.AspNetCore.Mvc;
using PSI.Services;
using PSI.Models;

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
        public async Task<IActionResult> PlaySong(Guid songId)
        {
            var result = await _songService.GetSongStreamAsync(songId);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            if (result.Stream == null)
            {
                return NotFound("Stream could not be opened.");
            }

            return File(result.Stream, result.ContentType, result.FileName);
        }
    }
}
