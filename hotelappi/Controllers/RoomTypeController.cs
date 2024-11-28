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


		//needs refactoring
		[Authorize]
		[HttpPost("{id}")]
		public async Task<ActionResult<RoomFullModel>> GetRoomTypeById(int id, [FromBody] FullRoomRequestModel model)
		{
			try
			{
				var fullRoomInfo = await _db.GetFullRoomInfoAsync(id, model.startDate, model.endDate);
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
					reviews
				});
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		//TODO
		//[HttpPost("filter")]
		//public async Task<ActionResult<List<RoomFullModel>>> GetAllRoomTypesById(int id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		//{
		//	try
		//	{
		//		var allRooms = await _db.GetAllTypesById(id, startDate, endDate);
		//		if (allRooms == null)
		//		{
		//			return NotFound();
		//		}
				

		//	}
		//	catch (Exception ex)
		//	{
		//		return BadRequest(ex.Message);
		//	}
		//}

		[HttpGet("available")]
		public async Task<ActionResult<List<RoomTypeModel>>> GetAvailableRoomTypes([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		{
			try
			{
				var availableRoomTypes = await _db.GetAvailableRoomTypesAsync(startDate, endDate);

				if (availableRoomTypes == null || !availableRoomTypes.Any())
				{
					return NotFound();
				}

				return Ok(availableRoomTypes);
			}
			catch (Exception ex)
			{

				return BadRequest(ex.Message);
			}
		}

		[HttpGet("allroomtypes")]
		public async Task<ActionResult<List<RoomTypeModel>>> GetAllRoomTypes()
		{
			try
			{
				var allRoomTypes = await _db.GetAllRoomTypesAsync();

				if (allRoomTypes == null || !allRoomTypes.Any())
				{
					return NotFound();
				}

				return Ok(allRoomTypes);
			}
			catch (Exception ex)
			{

				return BadRequest(ex.Message);
			}
		}
	}
}
