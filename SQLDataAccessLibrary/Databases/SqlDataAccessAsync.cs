﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Npgsql;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DataAccessLibrary.Databases
{
	public class SqlDataAccessAsync : ISqlDataAccessAsync
	{
		private readonly IConfiguration _config;

		public SqlDataAccessAsync(IConfiguration config)
		{
			_config = config;
		}

		public async Task<List<T>> LoadDataAsync<T, U>(string sqlStatement, U parameters, string connectionStringName, bool isStoredProcedure = false)
		{
			CommandType commandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;

			using (IDbConnection connection = new NpgsqlConnection(connectionStringName))
			{
				var rows = await connection.QueryAsync<T>(sqlStatement, parameters, commandType: commandType);
				return rows.ToList();
			}
		}

		public async Task<int> SaveDataAsync<T>(string sqlStatement, T parameters, string connectionStringName, bool isStoredProcedure = false)
		{ 
			CommandType commandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;

			using (IDbConnection connection = new NpgsqlConnection(connectionStringName))
			{
				return await connection.ExecuteAsync(sqlStatement, parameters, commandType: commandType);
			}
		}
	}
}
