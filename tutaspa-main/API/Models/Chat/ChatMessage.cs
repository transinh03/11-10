using ChatSupport.Hubs;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Chat
{
    public class ChatMessage
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string MessageId { get; set; } = Guid.NewGuid().ToString();
        public string SessionId { get; set; }
        public string FromUserId { get; set; }
        public string FromUserName { get; set; }
        public string? ToUserId { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime Timestamp { get; set; } =TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
        public bool IsFromAdmin { get; set; }
        public MessageStatus Status { get; set; } = MessageStatus.Sent;
        public bool IsRead { get; set; }

        // Navigation Property
        public virtual ChatSession ChatSession { get; set; }
    }
}
