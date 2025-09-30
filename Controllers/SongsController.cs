using Microsoft.AspNetCore.Mvc;
using PSI.Services;

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
    public IActionResult GetAll()
    {
        return Ok(songService.GetAll());
    }

    [HttpGet("{songId:guid}")]
    public IActionResult GetById(Guid songId)
    {
        var song = songService.GetById(songId);
        return song is not null ? Ok(song) : NotFound();
    }
}
