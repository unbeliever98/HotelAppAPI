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
		Task<RoomFullModel> GetFullRoomInfo(int id, DateTime startDate, DateTime endDate);
		Task<Dictionary<int, int>> GetFeaturePrices(List<int> featureIds);
		Task<int> GetRoomTypePrice(int id);
		Task<string> GetRoomNum(int id, DateTime startDate, DateTime endDate);
	}
}