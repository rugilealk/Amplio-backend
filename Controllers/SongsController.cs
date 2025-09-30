using Microsoft.AspNetCore.Mvc;
using PSI.Services;
using PSI.Models;

namespace PSI.Controllers
{
    [Route("songs")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly SongService songService;

        public SongsController(SongService songService)
        {
            this.songService = songService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSongs()
        {
            var allSongs = await songService.GetAllSongsAsync();
            return Ok(allSongs);
        }

        [HttpGet("{songId:guid}")]
        public async Task<IActionResult> GetSongById(Guid songId)
        {
            var song = await songService.GetSongByIdAsync(songId);
            return song is not null ? Ok(song) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSong([FromBody] SongDto songDto)
        {
            var createdSong = await songService.CreateSongAsync(songDto.Title, songDto.Artist);
            return Created($"/songs/{createdSong.Id}", createdSong);
        }

        public class SongDto
        {
            public string Title { get; set; } = string.Empty;
            public string Artist { get; set; } = string.Empty;
        }
    }
}
