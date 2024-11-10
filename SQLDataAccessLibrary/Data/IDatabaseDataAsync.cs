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
	}
}