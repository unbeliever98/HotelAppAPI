using Azure.Core;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using ProfanityFilter;


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

			var featureIdInts=new List<int>();

			var featurePrices = await _db.GetFeaturePricesAsync(bookingRequest.SelectedFeatures);
			var daysStaying = bookingRequest.EndDate.Subtract(bookingRequest.StartDate).Days;
			var basePrice = await _db.GetRoomTypePriceAsync(bookingRequest.Id);
			int totalPrice = basePrice * daysStaying + (bookingRequest.NumOfPeople > 1 ? 30 * (bookingRequest.NumOfPeople - 1) * daysStaying : 0);

			foreach (var featurePriceId in featurePrices)
			{
				if (featurePriceId.Key is 1 or 2 or 3)
				{
					totalPrice += featurePriceId.Value * bookingRequest.NumOfPeople * daysStaying;
				}
				else if (featurePriceId.Key is 4 or 5)
				{
					totalPrice += featurePriceId.Value;
				}
				featureIdInts.Add(featurePriceId.Key);
			}

			

			try
			{
				await _db.BookGuestAsync(userIdInt, bookingRequest.StartDate,
					bookingRequest.EndDate, bookingRequest.Id, totalPrice, bookingRequest.NumOfPeople);

				int bookingId = await _db.GetBookingIdAsync(userIdInt);

				if (featureIdInts.Count>1)
				{
					await _db.InsertFeaturesIntoBooking(bookingId, featureIdInts); 
				}
				return Ok(new { bookingId });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

		}

		[Authorize]
		[HttpPost("post-review")]
		public async Task<ActionResult> PostReview(ReviewRequestModel model)
		{
			var userIdString = User.FindFirst("UserId")?.Value;

			if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userIdInt))
			{
				return Unauthorized("Invalid token.");
			}

			var filter = new ProfanityFilter.ProfanityFilter();
			var censoredComment = filter.CensorString(model.Comment);

			var user = await _db.GetGuestByIdAsync(userIdInt);
			var bookingId= await _db.GetBookingIdAsync(userIdInt);

			var reviewExists = await _db.CheckIfReviewForBookingExistsAsync(bookingId);

			if (reviewExists == false)
			{
				await _db.PostReviewAsync(userIdInt, censoredComment, model.Rating);

				return Ok(new { censoredComment, model.Rating, user.FirstName, DateTime.Now });
			}else
			{
				return NotFound("Review already posted!");
			}

		}

		[Authorize]
		[HttpGet("{id}")]
		public async Task<ActionResult> GetReceipt(int id)
		{
			var userIdString = User.FindFirst("UserId")?.Value;

			if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userIdInt))
			{
				return Unauthorized("Invalid token.");
			}

			var result=await _db.GetFullBookingInfo(id, userIdInt);

			if (result != null)
			{
				var daysStaying = result.EndDate.Subtract(result.StartDate).Days;
				var pricePerNight = result.TotalPrice / daysStaying;

				bool isExpired = DateTime.Now.Date > result.EndDate;

				return Ok(new {bookingId=result.Id, result.StartDate, result.EndDate, result.NumOfPeople, result.TotalPrice, result.RoomNum, roomTitle=result.Title, result.Description, result.Image, result.FeatureNames, result.FeaturePrices, pricePerNight, isExpired});
			}
			else
			{
				return NotFound("This reciept does not belong you or doesn't exist.");
			}

		}

		[Authorize]
		[HttpDelete("delete-booking")]
		public async Task<ActionResult> DeleteBooking(int bookingId)
		{
			var userIdString = User.FindFirst("UserId")?.Value;

			if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userIdInt))
			{
				return Unauthorized("Invalid token.");
			}

			var result = await _db.DeleteBooking(bookingId, userIdInt);

			if (result > 0)
			{
				return Ok("Booking deleted successfully!");
			}
			else
			{
				return NotFound("No booking found or you are not authorized to delete it!");
			}
		}

		[Authorize]
		[HttpGet("search-bookings")]
		public async Task<ActionResult> GetUserBookings()
		{
			var userIdString = User.FindFirst("UserId")?.Value;

			if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userIdInt))
			{
				return Unauthorized("Invalid token.");
			}

			var result= await _db.GetPartialBookingInfo(userIdInt);

			if (result.Count > 0)
			{
				var bookings = result.Select(booking => new
				{
					bookingId = booking.Id,
					startDate = booking.StartDate,
					endDate = booking.EndDate,
					numOfPeople = booking.NumOfPeople,
					totalPrice = booking.TotalPrice,
					roomTitle = booking.Title,
					roomDescription = booking.Description,
					image = booking.Image,
					isExpired = DateTime.Now.Date > booking.EndDate,
				}).ToList();

				return Ok(new { bookings });
			}
			else
			{
				return NotFound("You have no bookings yet!");
			}
		}
	}
}
