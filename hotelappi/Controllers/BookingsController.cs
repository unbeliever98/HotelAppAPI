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

			var featurePrices = await _db.GetFeaturePricesAsync(bookingRequest.SelectedFeatures.ToArray());
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

				var bookingIds = await _db.GetBookingIdAsync(userIdInt);
				var bookingId = bookingIds.FirstOrDefault();

				if (featureIdInts.Count>1)
				{
					await _db.InsertFeaturesIntoBooking(bookingId, featureIdInts.ToArray()); 
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

			var reviewExists = await _db.CheckIfReviewForBookingExistsAsync(model.BookingId);

			if (reviewExists == false)
			{
				await _db.PostReviewAsync(model.BookingId, censoredComment, model.Rating);

				return Ok(new { comment=censoredComment, model.Rating, user.FirstName, uploadDate=DateTime.Now });
			}else
			{
				return NotFound("Review already posted!");
			}

		}

		[AllowAnonymous]
		[HttpGet("all-reviews")]
		public async Task<ActionResult> GetAllReviews()
		{
			var reviews = await _db.GetAllReviews();
			double sum = 0;
			double average = 0;

			foreach (var review in reviews)
			{
				sum += review.Rating;
			}
			
			if(reviews.Count > 0)
			{
				average = sum / reviews.Count;
			}	

			return Ok(new { reviews = reviews.OrderByDescending(x => x.CreatedAt), average });
		}

		[Authorize]
		[HttpPut("edit-review")]
		public async Task<ActionResult> UpdateReview(ReviewRequestModel model)
		{
			var userIdString = User.FindFirst("UserId")?.Value;

			if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userIdInt))
			{
				return Unauthorized("Invalid token.");
			}

			var bookings = await _db.GetBookingIdAsync(userIdInt);

			var filter = new ProfanityFilter.ProfanityFilter();
			var censoredComment = filter.CensorString(model.Comment);

			if (bookings.Contains(model.BookingId))
			{
				await _db.UpdateReview(model.BookingId, censoredComment, model.Rating);
				return Ok("Review updated successfully!");
			}
			else
			{
				return NotFound("This review does not belong to you!");
			}

			
		}

		[Authorize]
		[HttpDelete("delete-review")]
		public async Task<ActionResult> DeleteReview(int bookingId)
		{
			var userIdString = User.FindFirst("UserId")?.Value;

			if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userIdInt))
			{
				return Unauthorized("Invalid token.");
			}

			var bookings = await _db.GetBookingIdAsync(userIdInt);

			if (bookings.Contains(bookingId))
			{
				await _db.DeleteReview(bookingId);
				return Ok("Review deleted successfully!");
			}
			else
			{
				return NotFound("This review does not belong to you!");
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
			var basePrice = await _db.GetRoomTypePriceByBookingIdAsync(id);

			var result=await _db.GetFullBookingInfo(id, userIdInt);
			var reviewExists = await _db.CheckIfReviewForBookingExistsAsync(id);
			var review = await _db.GetUserReviewById(id);

			if (result != null)
			{
				var daysStaying = result.EndDate.Subtract(result.StartDate).Days;
				var pricePerNight = result.TotalPrice / daysStaying;

				bool isExpired = DateTime.Now.Date > result.EndDate;

				return Ok(new {bookingId=result.Id, result.StartDate, result.EndDate, result.NumOfPeople, basePrice, result.TotalPrice, result.RoomNum, roomTitle=result.Title, result.Description, result.Image, result.FeatureNames, result.FeaturePrices, pricePerNight, isExpired, reviewExists, review});
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

			return Ok("Booking deleted successfully!");

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
				var bookings = result
					.OrderBy(booking => booking.StartDate)
					.Select(booking => new
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

				return Ok( bookings);
			}
			else
			{
				return NotFound("You have no bookings yet!");
			}
		}


	}
}
