using DataAccessLibrary.Models;

namespace DataAccessLibrary.Data
{
	public interface IDatabaseDataAsync
	{
		Task BookGuestAsync(int id, DateTime startDate, DateTime endDate, int roomTypeId, int totalCost, int numOfPeople);
		Task<List<RoomTypeModel>> GetAvailableRoomTypesAsync(DateTime startDate, DateTime endDate);
		Task<RoomTypeModel> GetRoomTypeByIdAsync(int id);
		Task<List<RoomTypeModel>> GetAllRoomTypesAsync();
		Task RegisterGuestAsync(string firstName, string lastName, string email, string password);
		Task<List<string>> GetAllGuestEmailsAsync();
		Task<GuestModel> GetGuestInfoAsync(string email);
		Task<GuestModel> GetGuestByIdAsync(int id);
		Task ChangePasswordAsync(int id, string passwordHash);
		Task ChangeUserActivityAsync(int id);
		Task<RoomFullModel> GetFullRoomInfoAsync(int id, DateTime startDate, DateTime endDate);
		Task<Dictionary<int, int>> GetFeaturePricesAsync(int[] featureIds);
		Task<int> GetRoomTypePriceAsync(int id);
		Task<int> GetRoomTypePriceByBookingIdAsync(int bookingId);
		Task<string> GetRoomNumAsync(int id, DateTime startDate, DateTime endDate);
		Task PostReviewAsync(int bookingId, string comment, int rating);
		Task<List<int>> GetBookingIdAsync(int guestId);
		Task<bool> CheckIfReviewForBookingExistsAsync(int id);
		Task InsertFeaturesIntoBooking(int bookingId, int[] featureIds);
		Task<BookingFullModel> GetFullBookingInfo(int bookingId, int guestId);
		Task<int> DeleteBooking(int bookingId, int guestId);
		Task<List<BookingFullModel>> GetPartialBookingInfo(int guestId);
		Task<List<RoomFullModel>> GetAllTypesById(int id, DateTime startDate, DateTime endDate);
		Task<List<ReviewsFullModel>> GetAllUserReviews(int roomTypeId);
		Task UpdateReview(int bookingId, string comment, int rating);
		Task<ReviewsFullModel> GetUserReviewById(int bookingId);
		Task DeleteReview(int bookingId);
		Task<List<ReviewsFullModel>> GetAllReviews();
	}
}