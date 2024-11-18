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
		public async Task<ActionResult<RoomFullModel>> GetRoomTypeById(int id, [FromBody] FullRoomRequestModel requestModel)
		{
			try
			{
				var fullRoomInfo = await _db.GetFullRoomInfo(id, requestModel.startDate, requestModel.endDate);
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

				var dynamicFeatures = new Dictionary<string, bool>
				{
					{ "HasBreakfast", fullRoomInfo.HasBreakfast },
					{ "HasSauna", fullRoomInfo.HasSauna },
					{ "HasGym", fullRoomInfo.HasGym },
					{ "HasLaundryService", fullRoomInfo.HasLaundryService },
					{ "HasParking", fullRoomInfo.HasParking }
				};

				return Ok(new
				{
					fullRoomInfo.Id,
					fullRoomInfo.Description,
					fullRoomInfo.Price,
					fullRoomInfo.Image,
					fullRoomInfo.Rating,
					fullRoomInfo.MaxOccupancy,
					fullRoomInfo.FullDescription,
					fullRoomInfo.RoomId,
					StaticFeatures = staticFeatures,
					DynamicFeatures = dynamicFeatures
				});
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


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
