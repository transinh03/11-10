using API.Extensions;
using ChatSupport.Hubs;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TutaSpa.API.IService;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IMemoryCache _cache;

        public ChatController(IChatService chatService, IMemoryCache cache)
        {
            _chatService = chatService;
            _cache = cache;
        }

        [HttpGet("recent-sessions")]
        [Authorize(AuthenticationSchemes = "Bearer" , Roles = "Cashier")]
        public async  Task<IActionResult> GetRecentSession([FromQuery]int hours = 24, int pageSize = 50)
        {
            try
            {
                var adminId = User.GetUserId();
                var recentSessions = await _chatService.GetRecentSessionsAsync(adminId, hours, pageSize);

                var sessionList = recentSessions.Select(session => new
                {
                    session.SessionId,
                    session.CustomerId,
                    session.CustomerName,
                    session.AdminId,
                    session.AdminName,
                    session.StartTime,
                    session.EndTime,
                    Status = session.Status.ToString(),
                    //IsOnline = _userSessions.ContainsKey(session.CustomerId) && _userSessions[session.CustomerId].IsOnline,
                    LastMessage = session.Messages?.LastOrDefault()?.Message ?? "",
                    LastMessageTime = session.Messages?.LastOrDefault()?.Timestamp ?? session.StartTime,
                    MessageCount = session.Messages?.Count ?? 0
                }).ToList();

                return Ok(sessionList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách phiên chat: {ex.Message}");
                return NotFound(); 
            }
        }
        [HttpGet("history")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Cashier")]
        public async Task<IActionResult> GetChatHistory(string sessionId, int pageSize = 50, int pageNumber = 1)
        {
            try
            {
                var session = await _chatService.GetSessionByIdAsync(sessionId);
                var messages = await _chatService.GetChatHistoryAsync(sessionId, pageSize, pageNumber);

                var history = new
                {
                    CustomerId = session.CustomerId,
                    CustomerName = session.CustomerName,
                    SessionId = session.SessionId,
                    StartTime = session.StartTime,
                    Messages = messages.Select(m => new
                    {
                        m.MessageId,
                        m.FromUserId,
                        m.FromUserName,
                        m.ToUserId,
                        m.Message,
                        m.Timestamp,
                        m.IsFromAdmin,
                        m.Status,
                        m.IsRead
                    }).ToList(),
                    PageInfo = new
                    {
                        PageSize = pageSize,
                        PageNumber = pageNumber,
                        HasMore = messages.Count == pageSize
                    }
                };

                return Ok(history);
            }
            catch (Exception ex)
            {
               return NotFound(new { Message = "Không tìm thấy phiên chat", Error = ex.Message });
            }
        }
    }
}
