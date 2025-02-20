﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
	public class RoomTypeModel
	{
        public int Id { get; set; }
        public string Title { get; set; }
		public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string FullDescription { get; set; }
        public int MaxOccupancy { get; set; }
        public int AvailableRoomsCount { get; set; }
        public double AverageRating { get; set; }

    }
}
