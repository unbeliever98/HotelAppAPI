using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
	public class RoomModel
	{
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomNum { get; set; }
    }
}
