﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
	public class ReviewRequestModel
	{
        public int BookingId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}
