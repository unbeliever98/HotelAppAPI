using DataAccessLibrary.Databases;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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


		public async Task BookGuestAsync(int id, DateTime startDate, DateTime endDate, int roomTypeId, int totalCost, int numOfPeople)
		{

			GuestModel guest = (await _db.LoadDataAsync<GuestModel, dynamic>("dbo.spGuests_GetGuestById",
													   new { id },
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
							totalCost,
							numOfPeople
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

		public async Task<List<RoomTypeModel>> GetAllRoomTypesAsync()
		{
			return await _db.LoadDataAsync<RoomTypeModel, dynamic>("dbo.spRoomTypes_GetAllRoomTypes",
														  new { },
														  connectionStringName,
														  true);
		}

		public async Task RegisterGuestAsync(string firstName, string lastName, string email, string password)
		{

			await _db.SaveDataAsync("dbo.spGuests_Register", new
			{
				firstName,
				lastName,
				email,
				password
			}, connectionStringName, true);

		}

		public async Task<List<string>> GetAllGuestEmailsAsync()
		{
			return await _db.LoadDataAsync<string, dynamic>("dbo.spGuests_GetEmails", new { }, connectionStringName, true);		
		}

		public async Task<GuestModel> GetGuestInfoAsync(string email)
		{
			return (await _db.LoadDataAsync<GuestModel, dynamic>("dbo.spGuests_GetEmail", new {email}, connectionStringName, true)).FirstOrDefault();
		}

		public async Task<GuestModel> GetGuestByIdAsync(int id)
		{
			return (await _db.LoadDataAsync<GuestModel, dynamic>("dbo.spGuests_GetGuestById", new { id }, connectionStringName, true)).FirstOrDefault();
		}

		public async Task ChangePasswordAsync(int id, string passwordHash)
		{
			await _db.SaveDataAsync("dbo.SpGuests_UpdatePassword", new { passwordHash, id }, connectionStringName, true);
		}

		public async Task ChangeUserActivityAsync(int id)
		{
			await _db.SaveDataAsync("dbo.spGuests_ChangeActivity", new { id }, connectionStringName, true);
		}

		public async Task<RoomFullModel> GetFullRoomInfo(int id, DateTime startDate, DateTime endDate)
		{
			return (await _db.LoadDataAsync<RoomFullModel, dynamic>("dbo.spRoomTypes_GetByIdWithFeatures", new { id, startDate, endDate }, connectionStringName, true)).FirstOrDefault();
		}

		public async Task<Dictionary<int, int>> GetFeaturePrices(List<int> featureIds)
		{
			var featureIdString=string.Join(",",featureIds);

			return (await _db.LoadDataAsync <KeyValuePair<int, int>, dynamic>("dbo.spRoomFeatures_GetPriceById", new {ids=featureIdString}, connectionStringName, true)).ToDictionary(x=> x.Key, x=>x.Value);
		}

		public async Task<int> GetRoomTypePrice(int id)
		{
			return (await _db.LoadDataAsync<int, dynamic>("dbo.spRoomType_GetPrice", new { id }, connectionStringName, true)).FirstOrDefault();
		}

		public async Task<string> GetRoomNum(int roomTypeId, DateTime startDate, DateTime endDate)
		{
			RoomModel availableRoom = (await _db.LoadDataAsync<RoomModel, dynamic>("dbo.spRooms_GetAvailableRooms",
																	 new { startDate, endDate, roomTypeId },
																	 connectionStringName,
																	 true)).First();
			return availableRoom.RoomNum;
		}
	}
}
