using System;
using System.Text.Json.Serialization;

namespace API.DTOs
{
    public class MessageDto
    {
        public int id { get; set; }
        public int senderId { get; set; }
        public string senderUsername { get; set; }
        public string senderPhotoUrl { get; set; }
        public int recipientId { get; set; }
        public string recipientUsername { get; set; }
        public string recipientPhotoUrl { get; set; }
        public string content { get; set; }
        public DateTime? dateRead { get; set; }
        public DateTime messageSent { get; set; }
        [JsonIgnore]
        public bool senderDeleted { get; set; }
        [JsonIgnore]
        public bool recipientDeleted { get; set; }
    }
}