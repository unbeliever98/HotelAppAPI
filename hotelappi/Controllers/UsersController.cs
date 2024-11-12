using BCrypt.Net;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotelappi.Controllers
{
	[Route("api/users")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IDatabaseDataAsync _db;

		public UsersController(IDatabaseDataAsync db)
        {
			_db = db;
		}

		[HttpPost]
		[Route("register")]
		public async Task <ActionResult> Register([FromBody]GuestDTO guest)
		{
			var allEmails=await _db.GetAllEmails();

			if (allEmails.Contains(guest.Email))
			{
				return BadRequest("Email already exists!");
			}
			
			var hashedPassword=BCrypt.Net.BCrypt.EnhancedHashPassword(guest.Password, 13);

			try
			{
				await _db.RegisterGuest(guest.FirstName, guest.LastName,guest.Email, hashedPassword);
				return Ok("Registered successfully!");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);			
			}
		}
    }
}
