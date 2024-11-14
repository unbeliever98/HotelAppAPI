using DataAccessLibrary.Models;

namespace DataAccessLibrary.Data
{
	public interface IDatabaseDataAsync
	{
		Task BookGuestAsync(string firstName, string lastName, DateTime startDate, DateTime endDate, int roomTypeId);
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
	}
}