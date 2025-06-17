using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRental.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string RecipientId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;
    }
}
