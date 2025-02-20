﻿using DataAccessLibrary.Databases;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
	public class SqlData : IDatabaseData
	{
		private readonly ISQLDataAccess _db;
		private const string connectionStringName = "SqlDb";

		public SqlData(ISQLDataAccess db)
		{
			_db = db;
		}

		public List<RoomTypeModel> GetAvailableRoomTypes(DateTime startDate, DateTime endDate)
		{
			return _db.LoadData<RoomTypeModel, dynamic>("dbo.spRoomTypes_GetAvailalbleTypes",
											   new { startDate, endDate },
											   connectionStringName,
											   true);
		}

		public void BookGuest(string firstName, string lastName, DateTime startDate, DateTime endDate, int roomTypeId)
		{
			
			GuestModel guest = _db.LoadData<GuestModel, dynamic>("dbo.spGuests_Insert",
													   new { firstName, lastName },
													   connectionStringName,
													   true).First();

			RoomTypeModel roomType = _db.LoadData<RoomTypeModel, dynamic>("select * from dbo.RoomType where Id=@Id",
															   new { Id = roomTypeId },
															   connectionStringName,
															   false).First();

			TimeSpan timeStaying = endDate.Date.Subtract(startDate.Date);

			List<RoomModel> availableRooms = _db.LoadData<RoomModel, dynamic>("dbo.spRooms_GetAvailableRooms",
																	 new { startDate, endDate, roomTypeId },
																	 connectionStringName,
																	 true);
			_db.SaveData("dbo.spBookings_Insert",
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

		public List<BookingFullModel> SearchBookings(string lastName)
		{
			return _db.LoadData<BookingFullModel, dynamic>("dbo.spBookings_Search",
										   new { lastName, startDate = DateTime.Now.Date },
										   connectionStringName,
										   true);
		}

		public void CheckIn(int bookingId)
		{
			_db.SaveData(
				"update dbo.Bookings set CheckedIn=1 where Id=@Id",
				new { Id = bookingId },
				connectionStringName,
				false);
		}

		public RoomTypeModel GetRoomTypeById(int id)
		{
			return _db.LoadData<RoomTypeModel, dynamic>("dbo.spRoomTypes_GetById",
											   new { id },
											   connectionStringName,
											   true).First();
		}

	}
}
