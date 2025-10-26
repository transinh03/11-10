using API.Data;
using API.Models.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace TutaSpa.API.IService
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public ChatService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        #region Session Management

        public async Task<ChatSession> CreateOrGetSessionAsync(string customerId, string customerName, string? customerIP = null, string? userAgent = null)
        {
            // Tìm session active hiện tại của customer
            var existingSession = await _context.ChatSessions
                .FirstOrDefaultAsync(s => s.CustomerId == customerId &&
                                    s.Status == ChatSessionStatus.Active);

            if (existingSession != null)
                return existingSession;

            // Tạo session mới
            var newSession = new ChatSession
            {
                CustomerId = customerId,
                CustomerName = customerName,
                CustomerIP = customerIP,
                UserAgent = userAgent,
                Status = ChatSessionStatus.Waiting
            };

            _context.ChatSessions.Add(newSession);
            await _context.SaveChangesAsync();

            return newSession;
        }

        public async Task<ChatSession?> GetSessionByIdAsync(string sessionId)
        {
            string cacheKey = $"ChatSession_{sessionId}";
            if (_cache.TryGetValue(cacheKey, out ChatSession cachedSession))
            {
                return cachedSession;
            }

            var session = await _context.ChatSessions
                .Include(s => s.Messages.OrderBy(m => m.Timestamp))
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session != null)
            {
                _cache.Set(cacheKey, session, TimeSpan.FromMinutes(5));
            }
            return session;
        }

        public async Task<ChatSession?> GetActiveSessionByCustomerIdAsync(string customerId)
        {
            return await _context.ChatSessions
                .Include(s => s.Messages.OrderBy(m => m.Timestamp))
                .FirstOrDefaultAsync(s => s.CustomerId == customerId &&
                                    s.Status == ChatSessionStatus.Active);
        }

       public async Task<bool> AssignAdminToSessionAsync(string sessionId, string adminId, string adminName)
        {
            var session = await _context.ChatSessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null) return false;

            session.AdminId = adminId;
            session.AdminName = adminName;
            session.Status = ChatSessionStatus.Assigned;
            session.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Xóa cache của session và danh sách active sessions
            _cache.Remove($"ChatSession_{sessionId}");
            _cache.Remove("ActiveChatSessions");

            _cache.Remove($"RecentSessions_{adminId}_72_50");

            

            return true;
        }

        public async Task<bool> CloseSessionAsync(string sessionId)
        {
            var session = await _context.ChatSessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null) return false;

            session.Status = ChatSessionStatus.Closed;
            session.EndTime = DateTime.UtcNow;
            session.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ChatSession>> GetActiveSessionsAsync()
        {
            string cacheKey = "ActiveChatSessions";
            if (_cache.TryGetValue(cacheKey, out List<ChatSession> cachedList))
            {
                return cachedList;
            }

            var sessions = await _context.ChatSessions
                .Where(s => s.Status == ChatSessionStatus.Active || s.Status == ChatSessionStatus.Assigned)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

            _cache.Set(cacheKey, sessions, TimeSpan.FromMinutes(2));
            return sessions;
        }

        public async Task<List<ChatSession>> GetSessionsByAdminIdAsync(string adminId, int pageSize = 50, int pageNumber = 1)
        {
            return await _context.ChatSessions
                .Where(s => s.AdminId == adminId)
                .OrderByDescending(s => s.StartTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(s => s.Messages.OrderByDescending(m => m.Timestamp).Take(1)) // Last message only
                .ToListAsync();
        }

        public async Task<List<ChatSession>> GetRecentSessionsAsync(string adminId, int hours = 24, int pageSize = 50)
        {
            string cacheKey = $"RecentSessions_{adminId}_{hours}_{pageSize}";
            if (_cache.TryGetValue(cacheKey, out List<ChatSession> cachedList))
            {
                return cachedList;
            }

            var cutoffTime = DateTime.UtcNow.AddHours(-hours);

            var sessions = await _context.ChatSessions
                .Where(s => s.StartTime >= cutoffTime && s.AdminId == adminId)
                .OrderByDescending(s => s.StartTime)
                .Take(pageSize)
                .Include(s => s.Messages.OrderByDescending(m => m.Timestamp).Take(1))
                .ToListAsync();

            _cache.Set(cacheKey, sessions, TimeSpan.FromMinutes(2));
            return sessions;
        }

        #endregion

        #region Message Management

        public async Task<ChatMessage> SaveMessageAsync(ChatMessage messagea, string sessionId)
        {
            Console.WriteLine("Đã lưu"); 
            var message = new ChatMessage
            {
                MessageId = messagea.MessageId,
                SessionId = sessionId,
                FromUserId = messagea.FromUserId,
                FromUserName = messagea.FromUserName,
                ToUserId = messagea.ToUserId,
                Message = messagea.Message,
                Timestamp = messagea.Timestamp,
                IsFromAdmin = messagea.IsFromAdmin,
                Status = MessageStatus.Sent
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            return message;
        }

        public async Task<List<ChatMessage>> GetChatHistoryAsync(string sessionId, int pageSize = 100, int pageNumber = 1)
        {
            return await _context.ChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderByDescending(m => m.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(m => m.Timestamp) // Sắp xếp lại theo thời gian tăng dần để hiển thị
                .ToListAsync();
        }

        public async Task<List<ChatMessage>> GetUnreadMessagesAsync(string sessionId, bool isFromAdmin)
        {
            return await _context.ChatMessages
                .Where(m => m.SessionId == sessionId &&
                           m.IsFromAdmin == isFromAdmin &&
                           !m.IsRead)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<bool> MarkMessagesAsReadAsync(string sessionId, string userId, bool isAdmin)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.SessionId == sessionId &&
                           m.IsFromAdmin != isAdmin && // Tin nhắn từ phía đối diện
                           !m.IsRead)
                .ToListAsync();

            if (!messages.Any()) return false;

            foreach (var message in messages)
            {
                message.IsRead = true;
                message.Status = MessageStatus.Read;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ChatMessage?> GetLastMessageAsync(string sessionId)
        {
            return await _context.ChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderByDescending(m => m.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetUnreadMessageCountAsync(string sessionId, bool isFromAdmin)
        {
            return await _context.ChatMessages
                .CountAsync(m => m.SessionId == sessionId &&
                               m.IsFromAdmin == isFromAdmin &&
                               !m.IsRead);
        }

        #endregion
    }
}