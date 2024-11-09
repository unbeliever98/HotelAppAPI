using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
	public static class ApiConfiguration
	{
		public static string BaseUrl { get; set; }

		public static void Initialize(IConfiguration config)
		{
			BaseUrl = config["ApiSettings:BaseUrl"];
		}
	}
}
