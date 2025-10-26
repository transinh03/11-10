using API.Models.Chat;

namespace TutaSpa.API.IService
{
     public interface IChatService
    {
        // Session Management
        Task<ChatSession> CreateOrGetSessionAsync(string customerId, string customerName, string? customerIP = null, string? userAgent = null);
        Task<ChatSession?> GetSessionByIdAsync(string sessionId);
        Task<ChatSession?> GetActiveSessionByCustomerIdAsync(string customerId);
        Task<bool> AssignAdminToSessionAsync(string sessionId, string adminId, string adminName);
        Task<bool> CloseSessionAsync(string sessionId);
        Task<List<ChatSession>> GetActiveSessionsAsync();
        Task<List<ChatSession>> GetSessionsByAdminIdAsync(string adminId, int pageSize = 50, int pageNumber = 1);

        // Message Management
        Task<ChatMessage> SaveMessageAsync(ChatMessage message, string sessionId);
        Task<List<ChatMessage>> GetChatHistoryAsync(string sessionId, int pageSize = 100, int pageNumber = 1);
        Task<List<ChatMessage>> GetUnreadMessagesAsync(string sessionId, bool isFromAdmin);
        Task<bool> MarkMessagesAsReadAsync(string sessionId, string userId, bool isAdmin);
        Task<ChatMessage?> GetLastMessageAsync(string sessionId);

        // Analytics & Reporting (for future use)
        Task<int> GetUnreadMessageCountAsync(string sessionId, bool isFromAdmin);
        Task<List<ChatSession>> GetRecentSessionsAsync(string adminId , int hours = 24, int pageSize = 50);
    }
}
