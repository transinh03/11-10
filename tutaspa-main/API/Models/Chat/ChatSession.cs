using ChatSupport.Hubs;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Chat
{
    public class ChatSession
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? AdminId { get; set; }
        public string? AdminName { get; set; }

        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }

        public ChatSessionStatus Status { get; set; }

        // Metadata
        public string? CustomerIP { get; set; }
        public string? UserAgent { get; set; }
        public string? MetadataJson { get; set; } // Store as JSON

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }


    public enum ChatSessionStatus
    {
        Active,
        Waiting,
        Assigned,
        Closed,
        Abandoned
    }

    public enum MessageStatus
    {
        Sent ,
        Delivered,
        Read,
        Failed
    }
}
