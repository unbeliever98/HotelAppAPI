using DataAccessLibrary.Databases;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
	public class SqlDataAsync : IDatabaseDataAsync
	{
		private readonly ISqlDataAccessAsync _db;
		private const string connectionStringName = "SqlDb";

		public SqlDataAsync(ISqlDataAccessAsync db)
		{
			_db = db;
		}

		public async Task<List<RoomTypeModel>> GetAvailableRoomTypesAsync(DateTime startDate, DateTime endDate)
		{
			return await _db.LoadDataAsync<RoomTypeModel, dynamic>("dbo.spRoomTypes_GetAvailalbleTypes",
											   new { startDate, endDate },
											   connectionStringName,
											   true);
		}

		public async Task BookGuestAsync(string firstName, string lastName, DateTime startDate, DateTime endDate, int roomTypeId)
		{

			GuestModel guest = (await _db.LoadDataAsync<GuestModel, dynamic>("dbo.spGuests_Insert",
													   new { firstName, lastName },
													   connectionStringName,
													   true)).First();

			RoomTypeModel roomType = (await _db.LoadDataAsync<RoomTypeModel, dynamic>("select * from dbo.RoomType where Id=@Id",
															   new { Id = roomTypeId },
															   connectionStringName,
															   false)).First();

			TimeSpan timeStaying = endDate.Date.Subtract(startDate.Date);

			List<RoomModel> availableRooms = await _db.LoadDataAsync<RoomModel, dynamic>("dbo.spRooms_GetAvailableRooms",
																	 new { startDate, endDate, roomTypeId },
																	 connectionStringName,
																	 true);
			await _db.SaveDataAsync("dbo.spBookings_Insert",
						new
						{
							roomId = availableRooms.First().Id,
							guestId = guest.Id,
							startDate,
							endDate,
							totalCost = timeStaying.Days * roomType.Price
						},
						connectionStringName,
						true);
		}

		public async Task<List<BookingFullModel>> SearchBookingsAsync(string lastName)
		{
			return await _db.LoadDataAsync<BookingFullModel, dynamic>("dbo.spBookings_Search",
										   new { lastName, startDate = DateTime.Now.Date },
										   connectionStringName,
										   true);
		}

		public async Task CheckInAsync(int bookingId)
		{
			await _db.SaveDataAsync(
				"update dbo.Bookings set CheckedIn=1 where Id=@Id",
				new { Id = bookingId },
				connectionStringName,
				false);
		}

		public async Task<RoomTypeModel> GetRoomTypeByIdAsync(int id)
		{
			return (await _db.LoadDataAsync<RoomTypeModel, dynamic>("dbo.spRoomTypes_GetById",
											   new { id },
											   connectionStringName,
											   true)).First();
		}
	}
}
