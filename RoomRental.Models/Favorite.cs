using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRental.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;
    }
}
