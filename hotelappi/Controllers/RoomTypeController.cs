using DataAccessLibrary.Data;
using DataAccessLibrary.Databases;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementApp.Web.Controllers
{
	[Route("api/roomtypes")]
	[ApiController]
	public class RoomTypeController : ControllerBase
	{
		private readonly IDatabaseDataAsync _db;

		public RoomTypeController(IDatabaseDataAsync db)
		{
			_db = db;
		}

		[Authorize]
		[HttpPost("{id}")]
		public async Task<ActionResult<RoomFullModel>> GetRoomTypeById(int id, [FromBody] FullRoomRequestModel model)
		{
			var fullRoomInfo = await _db.GetFullRoomInfoAsync(id, model.StartDate, model.EndDate);
			if (fullRoomInfo == null)
			{
				return NotFound();
			}

			var staticFeatures = new Dictionary<string, bool>
				{
					{ "SeaView", fullRoomInfo.SeaView },
					{ "AirConditioning", fullRoomInfo.AirConditioning },
					{ "FreeWifi", fullRoomInfo.FreeWifi },
					{ "RoomService", fullRoomInfo.RoomService },
					{ "FlatScreenTv", fullRoomInfo.FlatScreenTv },
					{ "MiniFridge", fullRoomInfo.MiniFridge },
					{ "DailyHousekeeping", fullRoomInfo.DailyHousekeeping },
					{ "CoffeeMaker", fullRoomInfo.CoffeeMaker },
					{ "SafetyBox", fullRoomInfo.SafetyBox }
				};


			var reviews = await _db.GetAllUserReviews(fullRoomInfo.Id);


			return Ok(new
			{
				fullRoomInfo.Id,
				fullRoomInfo.Description,
				fullRoomInfo.StartDate,
				fullRoomInfo.EndDate,
				fullRoomInfo.Price,
				fullRoomInfo.Image,
				fullRoomInfo.MaxOccupancy,
				fullRoomInfo.FullDescription,
				fullRoomInfo.RoomId,
				StaticFeatures = staticFeatures,
				reviews = reviews.OrderByDescending(x => x.CreatedAt)
			});
		}

		[AllowAnonymous]
		[HttpGet("available")]
		public async Task<ActionResult<List<RoomTypeModel>>> GetAvailableRoomTypes([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		{
			var availableRoomTypes = await _db.GetAvailableRoomTypesAsync(startDate, endDate);

			return Ok(availableRoomTypes);

		}

		[AllowAnonymous]
		[HttpGet("allroomtypes")]
		public async Task<ActionResult<List<RoomTypeModel>>> GetAllRoomTypes()
		{
			var allRoomTypes = await _db.GetAllRoomTypesAsync();

			if (allRoomTypes == null || !allRoomTypes.Any())
			{
				return NotFound();
			}
			return Ok(allRoomTypes);
			

		}
	}
}
