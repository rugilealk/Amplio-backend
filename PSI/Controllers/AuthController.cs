using Microsoft.AspNetCore.Mvc;
using PSI.Services;

namespace PSI.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly AuthService _auth;

		public AuthController(AuthService auth)
		{
			_auth = auth;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] LoginRequest req)
		{
			var token = await _auth.Register(req.Username, req.Password);
			return Ok(new { token });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest req)
		{
			var token = await _auth.Login(req.Username, req.Password);
			return Ok(new { token });
		}
	}

	public class LoginRequest
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
