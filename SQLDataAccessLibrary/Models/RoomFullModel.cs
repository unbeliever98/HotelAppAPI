using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
	public class RoomFullModel
	{
        public int Id { get; set; }
        public int MaxOccupancy { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Image { get; set; }
        public int Rating { get; set; }
        public string FullDescription { get; set; }
        public int RoomId { get; set; }
        public bool SeaView { get; set; }
        public bool AirConditioning { get; set; }
        public bool FreeWifi { get; set; }
        public bool RoomService { get; set; }
        public bool FlatScreenTv { get; set; }
        public bool MiniFridge { get; set; }
        public bool DailyHousekeeping { get; set; }
        public bool CoffeeMaker { get; set; }
        public bool SafetyBox { get; set; }
        public bool HasBreakfast { get; set; } = false;
        public bool HasSauna { get; set; }=false;
        public bool HasGym { get; set; }=false;
        public bool HasLaundryService { get; set; } = false;
        public bool HasParking { get; set; } = false;
    }
}
