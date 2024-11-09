
namespace DataAccessLibrary.Databases
{
	public interface ISqlDataAccessAsync
	{
		Task<List<T>> LoadDataAsync<T, U>(string sqlStatement, U parameters, string connectionStringName, bool isStoredProcedure = false);
		Task SaveDataAsync<T>(string sqlStatement, T parameters, string connectionStringName, bool isStoredProcedure = false);
	}
}