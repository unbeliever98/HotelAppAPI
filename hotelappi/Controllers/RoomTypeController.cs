using DataAccessLibrary.Data;
using DataAccessLibrary.Databases;
using DataAccessLibrary.Models;
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

		[HttpGet("{id}")]
		public async Task<ActionResult<RoomTypeModel>> GetRoomTypeById(int id)
		{
			try
			{
				var roomType = await _db.GetRoomTypeByIdAsync(id);
				if (roomType == null)
				{
					return NotFound();
				}

				return Ok(roomType);
			}catch (Exception ex)
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

		[HttpGet ("allroomtypes")]
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
