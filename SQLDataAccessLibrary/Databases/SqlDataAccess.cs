﻿using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Databases
{
	public class SQLDataAccess : ISQLDataAccess
	{
		private readonly IConfiguration _config;

		public SQLDataAccess(IConfiguration config)
		{
			_config = config;
		}
		public List<T> LoadData<T, U>(string sqlStatement, U parameters, string connectionStringName, bool isStoredProcedure = false)
		{
			string connectionString = _config.GetConnectionString(connectionStringName);
			CommandType commandType = CommandType.Text;

			if (isStoredProcedure == true)
			{
				commandType = CommandType.StoredProcedure;
			}
		
			using (IDbConnection connection = new SqlConnection(connectionString))
			{
				List<T> rows = connection.Query<T>(sqlStatement, parameters, commandType: commandType).ToList();
				return rows;
			}
		}

		public void SaveData<T>(string sqlStatement, T parameters, string connectionStringName, bool isStoredProcedure = false)
		{
			string connectionString = _config.GetConnectionString(connectionStringName);
			CommandType commandType = CommandType.Text;

			if (isStoredProcedure==true)
			{
				commandType = CommandType.StoredProcedure;
			}

			using (IDbConnection connection = new SqlConnection(connectionString))
			{
				connection.Execute(sqlStatement, parameters, commandType: commandType);
			}
		}
	}
}
