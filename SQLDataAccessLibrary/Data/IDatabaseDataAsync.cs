using DataAccessLibrary.Models;

namespace DataAccessLibrary.Data
{
	public interface IDatabaseDataAsync
	{
		Task BookGuestAsync(int id, DateTime startDate, DateTime endDate, int roomTypeId, int totalCost, int numOfPeople);
		Task CheckInAsync(int bookingId);
		Task<List<RoomTypeModel>> GetAvailableRoomTypesAsync(DateTime startDate, DateTime endDate);
		Task<RoomTypeModel> GetRoomTypeByIdAsync(int id);
		Task<List<BookingFullModel>> SearchBookingsAsync(string lastName);
		Task<List<RoomTypeModel>> GetAllRoomTypesAsync();
		Task RegisterGuestAsync(string firstName, string lastName, string email, string password);
		Task<List<string>> GetAllGuestEmailsAsync();
		Task<GuestModel> GetGuestInfoAsync(string email);
		Task<GuestModel> GetGuestByIdAsync(int id);
		Task ChangePasswordAsync(int id, string passwordHash);
		Task ChangeUserActivityAsync(int id);
		Task<RoomFullModel> GetFullRoomInfoAsync(int id, DateTime startDate, DateTime endDate);
		Task<Dictionary<int, int>> GetFeaturePricesAsync(List<int> featureIds);
		Task<int> GetRoomTypePriceAsync(int id);
		Task<string> GetRoomNumAsync(int id, DateTime startDate, DateTime endDate);
		Task PostReviewAsync(int bookingId, string comment, int rating);
		Task<List<int>> GetBookingIdAsync(int guestId);
		Task<bool> CheckIfReviewForBookingExistsAsync(int id);
		Task InsertFeaturesIntoBooking(int bookingId, List<int> featureIds);
		Task<BookingFullModel> GetFullBookingInfo(int bookingId, int guestId);
		Task<int> DeleteBooking(int bookingId, int guestId);
		Task<List<BookingFullModel>> GetPartialBookingInfo(int guestId);
		Task<List<RoomFullModel>> GetAllTypesById(int id, DateTime startDate, DateTime endDate);
		Task<List<ReviewsFullModel>> GetAllUserReviews(int roomTypeId);
	}
}