using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PSI.Data;
using PSI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PSI.Services
{
	public class AuthService
	{
		private readonly AppDbContext _context;
		private readonly IConfiguration _config;

		public AuthService(AppDbContext context, IConfiguration config)
		{
			_context = context;
			_config = config;
		}

		public async Task<string> Register(string username, string password)
		{
			if (await _context.Users.AnyAsync(u => u.Username == username))
				throw new Exception("Username exists");

			CreatePasswordHash(password, out byte[] hash, out byte[] salt);

			var user = new User
			{
				Username = username,
				PasswordHash = hash,
				PasswordSalt = salt
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return CreateToken(user);
		}

		public async Task<string> Login(string username, string password)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
			if (user == null) throw new Exception("User not found");

			if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
				throw new Exception("Wrong password");

			return CreateToken(user);
		}

		private string CreateToken(User user)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Username)
			};

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_config["Jwt:Key"])
			);
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.UtcNow.AddHours(12),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
		{
			using var hmac = new HMACSHA256();
			salt = hmac.Key;
			hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
		}

		private bool VerifyPasswordHash(string pass, byte[] hash, byte[] salt)
		{
			using var hmac = new HMACSHA256(salt);
			var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(pass));
			return computed.SequenceEqual(hash);
		}
	}
}
