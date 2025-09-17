using Microsoft.AspNetCore.Mvc;
using PSI.Services;

[Route("songs")]
[ApiController]
public class SongsController : ControllerBase
{
    private readonly SongService _svc;

    public SongsController(SongService svc)
    {
        _svc = svc;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_svc.GetAll());
    }

    [HttpGet("{songId:guid}")]
    public IActionResult GetById(Guid songId)
    {
        var song = _svc.GetById(songId);
        return song is not null ? Ok(song) : NotFound();
    }
}
