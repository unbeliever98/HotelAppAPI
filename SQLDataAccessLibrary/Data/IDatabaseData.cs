using DataAccessLibrary.Models;

namespace DataAccessLibrary.Data
{
	public interface IDatabaseData
	{
		void BookGuest(string firstName, string lastName, DateTime startDate, DateTime endDate, int roomTypeId);
		void CheckIn(int bookingId);
		List<RoomTypeModel> GetAvailableRoomTypes(DateTime startDate, DateTime endDate);
		RoomTypeModel GetRoomTypeById(int id);
		List<BookingFullModel> SearchBookings(string lastName);
	}
}