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

		[HttpPost("register")]
		public async Task <ActionResult> Register([FromBody]GuestDTO guest)
		{
			var allEmails=await _db.GetAllGuestEmailsAsync();

			if (allEmails.Contains(guest.Email))
			{
				return BadRequest("Email already exists!");
			}

			if (guest.Password != guest.ConfirmPassword)
			{
				return BadRequest("Password and Confirm Password do not match!");
			}

			var hashedPassword=BCrypt.Net.BCrypt.EnhancedHashPassword(guest.Password, 13);

			try
			{
				await _db.RegisterGuestAsync(guest.FirstName, guest.LastName,guest.Email, hashedPassword);
				return Ok(new {User=guest.FirstName, guest.Email});
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);			
			}
		}

		[HttpPost("login")]
		public async Task<ActionResult> Login([FromBody] LoginDTO login)
		{
			var user = await _db.GetGuestInfoAsync(login.Email);

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
				new Claim("UserId",user.Id.ToString())
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(_config["Jwt:Issuer"],
									_config["Jwt:Audience"],
									claims,
									expires: DateTime.UtcNow.AddMinutes(30),
									signingCredentials: signIn);

			string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

			if (user.IsActive == false)
			{
				return Ok(new
				{
					Message = "Your account is deactivated. Do you want to restore it?",
					user.IsActive,
					Token=tokenValue,
					CanRestore = true
				});
			}


			return Ok(new {Token=tokenValue, User=user.Email, user.FirstName, user.LastName, user.IsActive});
		}
		[Authorize]
		[HttpPut("update-password")]
		public async Task <ActionResult> UpdatePassword([FromBody] UpdatePasswordModel pwModel)
		{
			if (pwModel == null || string.IsNullOrEmpty(pwModel.CurrentPassword) || string.IsNullOrEmpty(pwModel.NewPassword))
			{
				return BadRequest("Current and new passwords are required.");
			}

			var userIdString = User.FindFirst("UserId")?.Value;
			if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userIdInt))
			{
				return Unauthorized("Invalid token.");
			}

			var user = await _db.GetGuestByIdAsync(userIdInt);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			bool isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(pwModel.CurrentPassword, user.PasswordHash);
			if (!isPasswordValid)
			{
				return BadRequest("Invalid current password.");
			}

			if (pwModel.ConfirmNewPassword != pwModel.NewPassword)
			{
				return BadRequest("New password and confirmed new password don't match!");
			}

			user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(pwModel.NewPassword);

			await _db.ChangePasswordAsync(userIdInt, user.PasswordHash);
			return Ok("Password changed successfully.");

		}

		[Authorize]
		[HttpPut("deactivate-account")]
		public async Task <ActionResult> DeactivateUser()
		{
			var userIdString = User.FindFirst("UserId")?.Value;
			if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userIdInt))
			{
				return Unauthorized("Invalid token.");
			}

			var user = await _db.GetGuestByIdAsync(userIdInt);

			if (user == null)
			{
				return NotFound("User not found.");
			}

			if (user.IsActive == false)
			{
				return BadRequest("Account is already inactive.");
			}

			await _db.ChangeUserActivityAsync(userIdInt);
			return Ok("Account deactivated succesffuly");
		}

		[Authorize]
		[HttpPut("restore-account")]
		public async Task<ActionResult> RestoreUser()
		{
			var userIdString = User.FindFirst("UserId")?.Value;
			if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userIdInt))
			{
				return Unauthorized("Invalid token.");
			}

			var user = await _db.GetGuestByIdAsync(userIdInt);

			if (user == null)
			{
				return NotFound("User not found.");
			}

			if (user.IsActive == true)
			{
				return BadRequest("Account is already active.");
			}

			await _db.ChangeUserActivityAsync(userIdInt);
			user = await _db.GetGuestByIdAsync(userIdInt);
			return Ok(new {message= "Account restored succesffuly",firstName =user.FirstName, lastName=user.LastName, email=user.Email, user.IsActive});
		}

		[Authorize]
		[HttpGet("get-user")]
		public async Task<ActionResult> GetUserFromToken()
		{
			var userIdString=User.FindFirst("UserId")?.Value;

			if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userIdInt))
			{
				return Unauthorized("Invalid token.");
			}

			var user = await _db.GetGuestByIdAsync(userIdInt);

			if (user == null)
			{
				return NotFound("User not found.");
			}

			var userInfo = new
			{
				user.FirstName,
				user.LastName,
				user.Email,
				user.IsActive
			};

			return Ok(userInfo);
		}
		[Authorize]
		[HttpGet("check-token")]
		public async Task<ActionResult> TokenValid()
		{
			var userIdString = User.FindFirst("UserId")?.Value;

			if (string.IsNullOrEmpty(userIdString))
			{
				return Unauthorized("Invalid token.");
			}
			else return Ok(new { message = "Token is valid" });
			
		}
	}
}
