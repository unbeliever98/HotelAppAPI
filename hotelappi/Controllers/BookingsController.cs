using Azure.Core;
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
		public async Task<ActionResult> BookRoom([FromBody] BookingRequestModel bookingRequest)
		{
			var userIdString = User.FindFirst("UserId")?.Value;

			if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userIdInt))
			{
				return Unauthorized("Invalid token.");
			}

			var featurePrices = await _db.GetFeaturePrices(bookingRequest.SelectedFeatures);
			var daysStaying = bookingRequest.EndDate.Subtract(bookingRequest.StartDate).Days;
			var basePrice = await _db.GetRoomTypePrice(bookingRequest.Id);
			int totalPrice = basePrice*daysStaying + (bookingRequest.NumOfPeople > 1 ? 30 * (bookingRequest.NumOfPeople-1)*daysStaying : 0);

			foreach (var featurePriceId in featurePrices)
			{
				switch (featurePriceId.Key)
				{
					case 1:
						totalPrice += featurePriceId.Value * bookingRequest.NumOfPeople * daysStaying;
						break;
					case 2:
						totalPrice += featurePriceId.Value * bookingRequest.NumOfPeople * daysStaying;
						break;
					case 3:
						totalPrice += featurePriceId.Value * bookingRequest.NumOfPeople*daysStaying;
						break;
					case 4:
						totalPrice += featurePriceId.Value;
						break;
					case 5:
						totalPrice += featurePriceId.Value;
						break;
				}
			}

			var roomType= await _db.GetRoomTypeByIdAsync(bookingRequest.Id);
			var roomNum = (await _db.GetRoomNum(bookingRequest.Id,bookingRequest.StartDate, bookingRequest.EndDate));

			try
			{
				await _db.BookGuestAsync(userIdInt, bookingRequest.StartDate,
					bookingRequest.EndDate, bookingRequest.Id, totalPrice, bookingRequest.NumOfPeople);

				return Ok(new { roomType.Description,bookingRequest.StartDate,bookingRequest.EndDate, roomNum, totalPrice});
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

		}


	}
}
