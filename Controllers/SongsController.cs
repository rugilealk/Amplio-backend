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
            var song = await _songService.GetSongByIdAsync(songId);
            if (song == null)
            {
                return NotFound();
            }

            var fullPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", song.FilePath.Value);
            fullPath = Path.GetFullPath(fullPath);
          
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("File not found" + fullPath);
            }

            var fileStream = song.OpenStream();
            var contentType = "audio/mpeg";
            return File(fileStream, contentType, Path.GetFileName(fullPath));
        }

    }
}
