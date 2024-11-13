using BCrypt.Net;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace hotelappi.Controllers
{
	[Route("api/users")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IDatabaseDataAsync _db;
		private readonly IConfiguration _config;

		public UsersController(IDatabaseDataAsync db, IConfiguration config)
        {
			_db = db;
			_config = config;
		}

		[HttpPost]
		[Route("register")]
		public async Task <ActionResult> Register([FromBody]GuestDTO guest)
		{
			var allEmails=await _db.GetAllGuestEmails();

			if (allEmails.Contains(guest.Email))
			{
				return BadRequest("Email already exists!");
			}
			
			var hashedPassword=BCrypt.Net.BCrypt.EnhancedHashPassword(guest.Password, 13);

			try
			{
				await _db.RegisterGuest(guest.FirstName, guest.LastName,guest.Email, hashedPassword);
				return Ok(new {User=guest.FirstName, guest.Email});
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);			
			}
		}

		[HttpPost]
		[Route("login")]
		public async Task<ActionResult> Login([FromBody] LoginDTO login)
		{
			var user = await _db.GetGuestInfo(login.Email);

			if (user == null || string.IsNullOrEmpty(user.Email))
			{
				return Unauthorized("Invalid email or password.");
			}

			bool isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(login.Password, user.PasswordHash);

			if (!isPasswordValid)
			{
				return Unauthorized("Invalid email or password.");
			}

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim("Email", user.Email.ToString()),
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(_config["Jwt:Issuer"],
									_config["Jwt:Audience"],
									claims,
									expires: DateTime.UtcNow.AddMinutes(10),
									signingCredentials: signIn);

			string tokenValue=new JwtSecurityTokenHandler().WriteToken(token);
			return Ok(new {Token=tokenValue, User=user.Email, user.FirstName});

		}
    }
}
