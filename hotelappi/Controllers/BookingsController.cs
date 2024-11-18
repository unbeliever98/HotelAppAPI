using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace HotelManagementApp.Web.Controllers
{
	[Route("api/bookings")]
	[ApiController]
	public class BookingsController : ControllerBase
	{
		private readonly IDatabaseDataAsync _db;

        public BookingsController(IDatabaseDataAsync db)
        {
			_db = db;			
        }

		[Authorize]
        [HttpPost("book")]
		public async Task <ActionResult> BookRoom([FromBody] BookingRequestModel bookingRequest)
		{
			var userIdString = User.FindFirst("UserId")?.Value;

			if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userIdInt))
			{
				return Unauthorized("Invalid token.");
			}

			try
			{
				await _db.BookGuestAsync(userIdInt, bookingRequest.StartDate,
					bookingRequest.EndDate, bookingRequest.RoomTypeId);

				return Ok(new { message = "Booking successful"});
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
			
		}
	}
}
