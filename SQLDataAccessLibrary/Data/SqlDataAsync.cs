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
		private readonly string connectionStringName;

		public SqlDataAsync(ISqlDataAccessAsync db, string connectionStringName)
		{
			_db = db;
            connectionStringName = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            this.connectionStringName = connectionStringName;
			
        }

		public async Task<List<RoomTypeModel>> GetAvailableRoomTypesAsync(DateTime startDate, DateTime endDate)
		{
			return await _db.LoadDataAsync<RoomTypeModel, dynamic>("select * from sproomtypes_getavailabletypes(@StartDate::date,@EndDate::date)",
											   new {StartDate= startDate.Date, EndDate=endDate.Date },
											   connectionStringName,
											   false);
		}


		public async Task BookGuestAsync(int id, DateTime startDate, DateTime endDate, int roomTypeId, int totalCost, int numOfPeople)
		{

			GuestModel guest = (await _db.LoadDataAsync<GuestModel, dynamic>("select * from spGuests_GetGuestById(@Id)",
													   new { Id=id },
													   connectionStringName,
													   false)).First();

			RoomTypeModel roomType = (await _db.LoadDataAsync<RoomTypeModel, dynamic>("select * from sproomtypes_getbyid(@Id)",
															   new { Id=roomTypeId },
															   connectionStringName,
															   false)).First();

			List<RoomModel> availableRooms = await _db.LoadDataAsync<RoomModel, dynamic>("select * from sprooms_getavailablerooms(@StartDate::date,@EndDate::date, @Id)",
																	 new { StartDate=startDate.Date, EndDate=endDate.Date, Id=roomTypeId },
																	 connectionStringName,
																	 false);
			await _db.SaveDataAsync("SELECT spbookings_insert(@RoomId, @GuestId, @StartDate::date, @EndDate::date, @TotalCost, @NumOfPeople);",
						new
						{
							RoomId = availableRooms.First().Id,
							GuestId = guest.Id,
							StartDate=startDate.Date,
							EndDate=endDate.Date,
							TotalCost=totalCost,
							NumOfPeople=numOfPeople
						},
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
			return await _db.LoadDataAsync<RoomTypeModel, dynamic>("select * from sproomtypes_getallroomtypes()",
														  new { },
														  connectionStringName,
														  false);
		}

        public async Task RegisterGuestAsync(string firstName, string lastName, string email, string passwordHash)
        {
        
            string sql = "SELECT * FROM spguests_register(@FirstName, @LastName, @Email, @PasswordHash) AS t(Id integer, FirstName varchar, LastName varchar);";

            await _db.SaveDataAsync(sql, new
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = passwordHash
            }, connectionStringName, false);
        }


        public async Task<List<string>> GetAllGuestEmailsAsync()
		{
			return await _db.LoadDataAsync<string, dynamic>("select * from spGuests_GetEmails();", new { }, connectionStringName, false);		
		}

		public async Task<GuestModel> GetGuestInfoAsync(string email)
		{
			return (await _db.LoadDataAsync<GuestModel, dynamic>("SELECT * FROM spguests_getemail(@Email);", new {Email=email}, connectionStringName, false)).FirstOrDefault();
		}

		public async Task<GuestModel> GetGuestByIdAsync(int id)
		{
			return (await _db.LoadDataAsync<GuestModel, dynamic>("select * from spGuests_GetGuestById(@Id)", new {Id=id }, connectionStringName, false)).FirstOrDefault();
		}

		public async Task ChangePasswordAsync(int id, string passwordHash)
		{
			await _db.SaveDataAsync("select SpGuests_UpdatePassword(@PasswordHash, @Id)", new { PasswordHash=passwordHash,Id=id }, connectionStringName, false);
		}

		public async Task ChangeUserActivityAsync(int id)
		{
			await _db.SaveDataAsync("select spGuests_ChangeActivity(@Id)", new { Id=id }, connectionStringName, false);
		}

		public async Task<RoomFullModel> GetFullRoomInfoAsync(int id, DateTime startDate, DateTime endDate)
		{
			return (await _db.LoadDataAsync<RoomFullModel, dynamic>("SELECT * FROM sproomtypes_getbyidwithfeatures(@Id, @StartDate::date, @EndDate::date);", new { Id= id, StartDate=startDate.Date, EndDate = endDate.Date }, connectionStringName, false)).FirstOrDefault();
		}

		public async Task<Dictionary<int, int>> GetFeaturePricesAsync(int[] featureIds)
		{

			return (await _db.LoadDataAsync <KeyValuePair<int, int>, dynamic>("select * from spRoomFeatures_GetPriceById(@Ids)", new {Ids=featureIds}, connectionStringName, false)).ToDictionary(x=> x.Key, x=>x.Value);
		}

		public async Task<int> GetRoomTypePriceAsync(int id)
		{
			return (await _db.LoadDataAsync<int, dynamic>("select * from spRoomType_GetPrice(@Id)", new { Id=id }, connectionStringName, false)).FirstOrDefault();
		}

		public async Task<int> GetRoomTypePriceByBookingIdAsync(int bookingId)
		{
			return (await _db.LoadDataAsync<int, dynamic>("select * from spRoomType_GetPriceByBookingId(@Id)", new {Id= bookingId }, connectionStringName, false)).FirstOrDefault();
		}

		public async Task<string> GetRoomNumAsync(int roomTypeId, DateTime startDate, DateTime endDate)
		{
			RoomModel availableRoom = (await _db.LoadDataAsync<RoomModel, dynamic>("dbo.spRooms_GetAvailableRooms",
																	 new { startDate, endDate, roomTypeId },
																	 connectionStringName,
																	 true)).First();
			return availableRoom.RoomNum;
		}

		public async Task PostReviewAsync(int bookingId, string comment, int rating)
		{ 
			await _db.SaveDataAsync("select spreviews_insertreview(@Id, @Comment,@Rating)", new {Id=bookingId, Comment=comment,Rating=rating}, connectionStringName, false);
		}

		public async Task UpdateReview(int bookingId, string comment, int rating)
		{
			await _db.SaveDataAsync("select spreviews_updatereview(@Id, @Comment, @Rating)", new { Id=bookingId, Comment=comment, Rating=rating }, connectionStringName, false);
		}

		public async Task<List<ReviewsFullModel>> GetAllUserReviews(int roomTypeId)
		{
			return await _db.LoadDataAsync<ReviewsFullModel, dynamic>("select * from spreviews_getuserreviews(@Id)", new { Id=roomTypeId }, connectionStringName, false);
		}

		public async Task<ReviewsFullModel> GetUserReviewById(int bookingId)
		{
			return (await _db.LoadDataAsync<ReviewsFullModel, dynamic>("select * from spReviews_GetUserReviewsById (@Id)", new {Id= bookingId }, connectionStringName, false)).FirstOrDefault();
		}

		public async Task<bool> CheckIfReviewForBookingExistsAsync(int bookingId)
		{
			var results = await _db.LoadDataAsync<int, dynamic>("select * from spReviews_CheckForBookingId(@Id)", new {Id=bookingId}, connectionStringName, false);

			if(results.Count > 0)
			{
				return true;
			}
			return false;
		}

		public async Task<List<ReviewsFullModel>> GetAllReviews()
		{
			return await _db.LoadDataAsync<ReviewsFullModel, dynamic>("select * from spreviews_getallreviews()", new { }, connectionStringName, false);
		}

		public async Task DeleteReview(int bookingId)
		{
			await _db.SaveDataAsync("select spReviews_DeleteReview(@Id)", new { Id=bookingId }, connectionStringName, false);
		}

		public async Task <List<int>> GetBookingIdAsync(int guestId)
		{
			return await _db.LoadDataAsync<int, dynamic>("select * from spbookings_getbookingbyguestid(@Id)", new {Id= guestId }, connectionStringName, false);
		}

		public async Task InsertFeaturesIntoBooking(int bookingId, int[] featureIds)
		{
			await _db.SaveDataAsync("select spBookingFeatures_InsertBookingFeature(@BookingId, @Ids)", new {BookingId=bookingId, Ids=featureIds},connectionStringName, false);
		}
		
		public async Task <BookingFullModel> GetFullBookingInfo(int bookingId, int guestId)
		{
			return (await _db.LoadDataAsync<BookingFullModel, dynamic>("select * from spbookings_getfullbookinginfo(@BookingId,@GuestId)", new { BookingId=bookingId, GuestId=guestId }, connectionStringName, false)).FirstOrDefault();
		}

		public async Task<int> DeleteBooking(int bookingId, int guestId)
		{
			return await _db.SaveDataAsync("select spbookings_deletebooking(@BookingId,@GuestId)", new { BookingId=bookingId,GuestId= guestId }, connectionStringName, false);
		}

		public async Task<List<BookingFullModel>> GetPartialBookingInfo(int guestId)
		{
			return await _db.LoadDataAsync<BookingFullModel, dynamic>("select * from spbookings_getpartialbookinginfo(@GuestId)", new { GuestId=guestId }, connectionStringName, false);
		}

		public async Task<List<RoomFullModel>> GetAllTypesById (int id, DateTime startDate, DateTime endDate)
		{
			return await _db.LoadDataAsync<RoomFullModel, dynamic>("dbo.spRoomTypes_GetAllTypesByIdWithFeatures", new { id, startDate, endDate }, connectionStringName, true);
		}
	}
}
