﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
	public class BookingRequestModel
	{
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RoomTypeId { get; set; }
    }
}
