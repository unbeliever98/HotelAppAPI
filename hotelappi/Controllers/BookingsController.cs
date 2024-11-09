using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace HotelManagementApp.Web.Controllers
{
	[Route("webapi/bookings")]
	[ApiController]
	public class BookingsController : ControllerBase
	{
		private readonly IDatabaseDataAsync _db;

        public BookingsController(IDatabaseDataAsync db)
        {
			_db = db;			
        }

        [HttpPost("book")]
		public async Task <ActionResult> BookRoom([FromBody] BookingRequestModel bookingRequest)
		{
            try
			{
				await _db.BookGuestAsync(bookingRequest.FirstName, bookingRequest.LastName, bookingRequest.StartDate,
					bookingRequest.EndDate, bookingRequest.RoomTypeId);

				return Ok(new { message = "Booking successful" });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
			
		}
	}
}
