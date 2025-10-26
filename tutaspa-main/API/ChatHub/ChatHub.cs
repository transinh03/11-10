using API.DTOs.Auth;
using API.Models;
using API.Models.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using TutaSpa.API.IService;

namespace ChatSupport.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private const int MaxUserAssignToAdmin = 2;

        // User session stores
        private static readonly ConcurrentDictionary<string, UserSession> _userSessions = new();
        private static readonly ConcurrentDictionary<string, string> _connectionToUser = new();

        // Admin session stores
        private static readonly ConcurrentDictionary<string, AdminSession> _adminSessions = new();
        // connectionId -> adminId
        private static readonly ConcurrentDictionary<string, string> _connectionToAdmin = new();

        // how long to wait before considering disconnect real (for refresh/F5)
        private static readonly TimeSpan DisconnectGrace = TimeSpan.FromSeconds(7);

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        #region Join / Init

        public async Task JoinAsGuest(string guestName)
        {
            var userId = Context.ConnectionId;
            var userName = string.IsNullOrEmpty(guestName) ? $"Khách #{userId[..8]}" : guestName;

            var dbSession = await _chatService.CreateOrGetSessionAsync(
                customerId: userId,
                customerName: userName,
                customerIP: Context.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString(),
                userAgent: Context.GetHttpContext()?.Request?.Headers["User-Agent"].ToString()
            );

            // Nếu phiên chat đã đóng thì không cho join
            if (dbSession.Status == ChatSessionStatus.Closed)
            {
                await Clients.Caller.SendAsync("ChatClosed", new { sessionId = dbSession.SessionId });
                return;
            }

            var session = new UserSession
            {
                UserId = userId,
                UserName = userName,
                ConnectionId = Context.ConnectionId,
                StartTime = DateTime.Now,
                IsOnline = true,
                SessionId = dbSession.SessionId
            };

            _userSessions.TryAdd(userId, session);
            _connectionToUser.TryAdd(Context.ConnectionId, userId);

            await Groups.AddToGroupAsync(Context.ConnectionId, "Customers");

            await TryAssignUserToAvailableAdmin(session);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Cashier")]
        public async Task JoinAsAdmin()
        {
            var adminId = GetAdminId();
            var adminName = GetAdminName();

            if (adminId == null || adminName == null)
            {
                await Clients.Caller.SendAsync("Error", "Phiên đăng nhập không hợp lệ");
                return;
            }

            // Nếu admin đã tồn tại, chỉ thêm connectionId (multi-tab support)
            if (_adminSessions.TryGetValue(adminId, out var existing))
            {
                existing.ConnectionIds.Add(Context.ConnectionId);
                _connectionToAdmin[Context.ConnectionId] = adminId;

                // join the admin connection to Admins group (so broadcast still works)
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");

                // try assign waiting users if this admin has capacity
                await AutoAssignWaitingUsersToAdmin(existing);

                // notify other admins (except this connection)
                await Clients.GroupExcept("Admins", Context.ConnectionId).SendAsync("AdminJoined", new
                {
                    AdminId = adminId,
                    AdminName = adminName
                });

                return;
            }

            // tạo session admin mới
            var adminSession = new AdminSession
            {
                AdminId = adminId,
                AdminName = adminName
            };
            adminSession.ConnectionIds.Add(Context.ConnectionId);

            _adminSessions.TryAdd(adminId, adminSession);
            _connectionToAdmin[Context.ConnectionId] = adminId;

            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");

            await AutoAssignWaitingUsersToAdmin(adminSession);

            await Clients.GroupExcept("Admins", Context.ConnectionId).SendAsync("AdminJoined", new
            {
                AdminId = adminId,
                AdminName = adminName
            });
        }

        #endregion

        #region Assignment / Messaging

        private async Task<bool> TryAssignUserToAvailableAdmin(UserSession user)
        {
            var availableAdmin = _adminSessions.Values
                .Where(a => a.AssignedUsers.Count < MaxUserAssignToAdmin)
                .OrderBy(a => a.AssignedUsers.Count)
                .FirstOrDefault();

            if (availableAdmin != null)
            {
                await AssignUserToAdmin(user, availableAdmin);

                await Clients.Caller.SendAsync("ChatSessionCreated", new
                {
                    SessionId = user.UserId,
                    UserName = user.UserName,
                    Message = $"Xin chào! Tôi là {availableAdmin.AdminName} rất vui được hỗ trợ bạn"
                });

                return true;
            }
            else if (_adminSessions.Count > 0)
            {
                await Clients.Group("Admins").SendAsync("NewCustomerJoined", new
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    JoinTime = user.StartTime,
                    AssignedAdminId = (string)null,
                    IsAssigned = false
                });

                await Clients.Caller.SendAsync("ChatSessionCreated", new
                {
                    SessionId = user.UserId,
                    UserName = user.UserName,
                    Message = $"Chào mừng {user.UserName}! Tất cả nhân viên hỗ trợ hiện đang bận. Hãy để lại tin nhắn và chúng tôi sẽ hỗ trợ bạn sớm nhất có thể."
                });
                return false;
            }
            else
            {
                await Clients.Caller.SendAsync("ChatSessionCreated", new
                {
                    SessionId = user.UserId,
                    UserName = user.UserName,
                    Message = $"Chào mừng {user.UserName}! Hiện tại chưa có nhân viên hỗ trợ online. Hãy để lại tin nhắn và chúng tôi sẽ phản hồi sớm nhất có thể."
                });
                return false;
            }
        }

        private async Task AssignUserToAdmin(UserSession user, AdminSession admin, AdminSession oldAdmin = null)
        {
            if (user.AssignedAdminId == admin.AdminId && admin.AssignedUsers.Contains(user.UserId))
                return;

            if (admin.AssignedUsers.Count >= MaxUserAssignToAdmin)
                return;

            // Nếu có oldAdmin thì remove ở đó
            if (!string.IsNullOrEmpty(user.AssignedAdminId) && user.AssignedAdminId != admin.AdminId)
            {
                if (_adminSessions.TryGetValue(user.AssignedAdminId, out var oldAdminSession))
                {
                    oldAdminSession.AssignedUsers.Remove(user.UserId);
                }
            }

            user.AssignedAdminId = admin.AdminId;
            if (!admin.AssignedUsers.Contains(user.UserId))
                admin.AssignedUsers.Add(user.UserId);

            await _chatService.AssignAdminToSessionAsync(user.SessionId, admin.AdminId, admin.AdminName);

            var chatMessage = new ChatMessage
            {
                FromUserId = admin.AdminId,
                FromUserName = admin.AdminName,
                ToUserId = user.UserId,
                Message = $"Xin chào! Tôi là {admin.AdminName}, rất vui được hỗ trợ bạn hôm nay.",
                IsFromAdmin = true
            };

            // Notify customer once when initial assignment
            await Clients.Client(user.ConnectionId).SendAsync("AdminAssigned", new
            {
                AdminId = admin.AdminId,
                AdminName = admin.AdminName,
                Message = $"Bạn đã được kết nối với {admin.AdminName}.",
                ChatMessage = chatMessage
            });

            await _chatService.SaveMessageAsync(chatMessage, user.SessionId);

            // Notify admin (all connections)
            foreach (var conn in admin.ConnectionIds.ToList())
            {
                await Clients.Client(conn).SendAsync("UserAssigned", new
                {
                    SessionId = user.SessionId,
                    CustomerId = user.UserId,
                    CustomerName = user.UserName,
                    StartTime = user.StartTime,
                    IsOnline = user.IsOnline,
                    LastMessage = "",
                    LastMessageTime = user.StartTime,
                    UnreadCount = user.WaitingMessages?.Count ?? 0,
                    WaitingMessages = user.WaitingMessages
                });
            }

            user.WaitingMessages.Clear();
        }

        private async Task AutoAssignWaitingUsersToAdmin(AdminSession admin)
        {
            var waitingUsers = _userSessions.Values
                .Where(u => u.IsOnline && string.IsNullOrEmpty(u.AssignedAdminId))
                .OrderBy(u => u.StartTime)
                .Take(MaxUserAssignToAdmin)
                .ToList();

            foreach (var user in waitingUsers)
            {
                if (admin.AssignedUsers.Count >= MaxUserAssignToAdmin)
                    break;

                await AssignUserToAdmin(user, admin);
                await Task.Delay(100);
            }
        }

        public async Task SendMessageToSupport(string message)
        {
            if (!_connectionToUser.TryGetValue(Context.ConnectionId, out var userId) ||
                !_userSessions.TryGetValue(userId, out var session))
            {
                await Clients.Caller.SendAsync("Error", "Phiên chat không hợp lệ");
                return;
            }

            var dbSession = await _chatService.GetSessionByIdAsync(session.SessionId);
            if (dbSession.Status == ChatSessionStatus.Closed)
            {
                await Clients.Caller.SendAsync("ChatClosed", new { sessionId = dbSession.SessionId });
                return;
            }

            var chatMessage = new ChatMessage
            {
                SessionId = session.SessionId,
                FromUserId = session.UserId,
                FromUserName = session.UserName,
                Message = message,
                IsFromAdmin = false
            };

            bool messageSentToAdmin = false;

            if (!string.IsNullOrEmpty(session.AssignedAdminId))
            {
                if (_adminSessions.TryGetValue(session.AssignedAdminId, out var assignedAdmin))
                {
                    chatMessage.ToUserId = assignedAdmin.AdminId;

                    await _chatService.SaveMessageAsync(chatMessage, session.SessionId);

                    foreach (var conn in assignedAdmin.ConnectionIds.ToList())
                    {
                        await Clients.Client(conn).SendAsync("ReceiveMessage", chatMessage);
                    }

                    messageSentToAdmin = true;
                }
            }

            if (!messageSentToAdmin)
            {
                await _chatService.SaveMessageAsync(chatMessage, session.SessionId);

                session.WaitingMessages.Add(chatMessage);

                if (_adminSessions.Count > 0)
                {
                    await Clients.Group("Admins").SendAsync("ReceiveMessage", chatMessage);
                }

                await Clients.Caller.SendAsync("MessageQueued", new
                {
                    Message = "Tin nhắn của bạn đã được ghi nhận và đang chờ xử lý."
                });
            }

            await Clients.Caller.SendAsync("MessageSent", chatMessage);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Cashier")]
        public async Task SendMessageToCustomer(string customerId, string message)
        {
            if (!_userSessions.TryGetValue(customerId, out var customerSession))
            {
                await Clients.Caller.SendAsync("Error", "Khách hàng không tồn tại hoặc đã offline");
                return;
            }

            var adminSession = _adminSessions.Values.FirstOrDefault(a => a.ConnectionIds.Contains(Context.ConnectionId));
            if (adminSession == null)
            {
                await Clients.Caller.SendAsync("Error", "Phiên admin không hợp lệ");
                return;
            }

            var dbSession = await _chatService.GetSessionByIdAsync(customerSession.SessionId);
            if (dbSession.Status == ChatSessionStatus.Closed)
            {
                await Clients.Caller.SendAsync("ChatClosed", new { sessionId = dbSession.SessionId });
                return;
            }

            var chatMessage = new ChatMessage
            {
                FromUserId = adminSession.AdminId,
                FromUserName = adminSession.AdminName,
                ToUserId = customerId,
                Message = message,
                IsFromAdmin = true
            };

            await _chatService.SaveMessageAsync(chatMessage, customerSession.SessionId);

            if (customerSession.IsOnline)
            {
                await Clients.Client(customerSession.ConnectionId).SendAsync("ReceiveMessage", chatMessage);
            }

            await Clients.Caller.SendAsync("MessageSent", chatMessage);
        }

        #endregion

        #region Queries

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Cashier")]
        public async Task GetOnlineCustomers()
        {
            var onlineCustomers = new List<object>();

            foreach (var userSession in _userSessions.Values.Where(u => u.IsOnline))
            {
                try
                {
                    var lastMessage = await _chatService.GetLastMessageAsync(userSession.SessionId);
                    var unreadCount = await _chatService.GetUnreadMessageCountAsync(userSession.SessionId, false);

                    onlineCustomers.Add(new
                    {
                        userSession.UserId,
                        userSession.UserName,
                        userSession.StartTime,
                        userSession.AssignedAdminId,
                        IsAssigned = !string.IsNullOrEmpty(userSession.AssignedAdminId),
                        LastMessage = lastMessage?.Message ?? "",
                        LastMessageTime = lastMessage?.Timestamp ?? userSession.StartTime,
                        UnreadCount = unreadCount,
                        SessionId = userSession.SessionId
                    });
                }
                catch (Exception)
                {
                    onlineCustomers.Add(new
                    {
                        userSession.UserId,
                        userSession.UserName,
                        userSession.StartTime,
                        userSession.AssignedAdminId,
                        IsAssigned = !string.IsNullOrEmpty(userSession.AssignedAdminId),
                        LastMessage = "Error loading message",
                        LastMessageTime = userSession.StartTime,
                        UnreadCount = 0,
                        SessionId = userSession.SessionId
                    });
                }
            }

            await Clients.Caller.SendAsync("OnlineCustomers", onlineCustomers.OrderByDescending(c => ((DateTime)c.GetType().GetProperty("StartTime").GetValue(c))).ToList());
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Cashier")]
        public async Task GetRecentSessions(int hours = 24, int pageSize = 50)
        {
            try
            {
                var adminId = GetAdminId();
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
                    IsOnline = _userSessions.ContainsKey(session.CustomerId) && _userSessions[session.CustomerId].IsOnline,
                    LastMessage = session.Messages?.LastOrDefault()?.Message ?? "",
                    LastMessageTime = session.Messages?.LastOrDefault()?.Timestamp ?? session.StartTime,
                    MessageCount = session.Messages?.Count ?? 0
                }).ToList();

                await Clients.Caller.SendAsync("RecentSessions", sessionList);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Lỗi khi lấy danh sách session: {ex.Message}");
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Cashier")]
        public async Task GetChatHistory(string sessionId, int pageSize = 50, int pageNumber = 1)
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
                    IsOnline = _userSessions.TryGetValue(session.CustomerId, out var userSession) && userSession.IsOnline,
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

                await Clients.Caller.SendAsync("ChatHistory", history);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Lỗi khi lấy lịch sử chat: {ex.Message}");
            }
        }

        #endregion

        #region Disconnect handling

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Nếu là customer disconnected
            if (_connectionToUser.TryRemove(Context.ConnectionId, out var userId))
            {
                if (_userSessions.TryGetValue(userId, out var session))
                {
                    session.IsOnline = false;

                    // Đợi ngắn để phân biệt F5/reconnect
                    await Task.Delay(DisconnectGrace);

                    // Nếu user reconnect (another connection added)
                    if (_userSessions.TryGetValue(userId, out var checkSession) && checkSession.IsOnline)
                        return;

                    // Khách thật sự offline -> đóng phiên ở DB
                    await _chatService.CloseSessionAsync(session.SessionId);

                    // Thông báo admin (nếu có)
                    if (!string.IsNullOrEmpty(session.AssignedAdminId) &&
                        _adminSessions.TryGetValue(session.AssignedAdminId, out var assignedAdmin))
                    {
                        assignedAdmin.AssignedUsers.Remove(userId);
                        foreach (var conn in assignedAdmin.ConnectionIds.ToList())
                        {
                            await Clients.Client(conn).SendAsync("CustomerDisconnected", new
                            {
                                sessionId = session.SessionId
                            });
                        }
                    }

                    await BroadcastCustomerListUpdate();
                }
            }

            // Nếu là admin disconnected
            if (_connectionToAdmin.TryRemove(Context.ConnectionId, out var adminId))
            {
                if (_adminSessions.TryGetValue(adminId, out var adminSession))
                {
                    // remove this connectionId from the admin
                    adminSession.ConnectionIds.Remove(Context.ConnectionId);

                    // wait short grace to avoid refresh
                    await Task.Delay(DisconnectGrace);

                    // if admin has other active connectionIds -> still online, nothing to do
                    if (adminSession.ConnectionIds.Count > 0)
                        return;

                    // Admin thật sự offline -> remove admin session and CLOSE sessions of assigned users immediately
                    _adminSessions.TryRemove(adminId, out _);

                    var orphanedCustomerIds = new List<string>(adminSession.AssignedUsers);

                    foreach (var customerId in orphanedCustomerIds)
                    {
                        if (_userSessions.TryGetValue(customerId, out var customerSession))
                        {
                            // Close DB session immediately because requirement: do not reassign to other admin
                            try
                            {
                                await _chatService.CloseSessionAsync(customerSession.SessionId);
                            }
                            catch
                            {
                                // optionally log error
                            }

                            // Notify customer if online
                            if (customerSession.IsOnline)
                            {
                                await Clients.Client(customerSession.ConnectionId).SendAsync("ChatClosed", new { sessionId = customerSession.SessionId });
                            }

                            // Update local structures
                            customerSession.AssignedAdminId = null;
                        }
                    }

                    // Notify other admins that this admin left
                    await Clients.Group("Admins").SendAsync("AdminLeft", new
                    {
                        AdminId = adminSession.AdminId,
                        AdminName = adminSession.AdminName
                    });

                    await BroadcastCustomerListUpdate();
                }
            }
            else
            {
                // fallback: try find admin session by scanning connectionIds (rare)
                var adminSession = _adminSessions.Values.FirstOrDefault(a => a.ConnectionIds.Contains(Context.ConnectionId));
                if (adminSession != null)
                {
                    adminSession.ConnectionIds.Remove(Context.ConnectionId);
                    _connectionToAdmin.TryRemove(Context.ConnectionId, out _);

                    await Task.Delay(DisconnectGrace);

                    if (adminSession.ConnectionIds.Count == 0)
                    {
                        _adminSessions.TryRemove(adminSession.AdminId, out _);

                        var orphanedCustomerIds = new List<string>(adminSession.AssignedUsers);
                        foreach (var customerId in orphanedCustomerIds)
                        {
                            if (_userSessions.TryGetValue(customerId, out var customerSession))
                            {
                                try
                                {
                                    await _chatService.CloseSessionAsync(customerSession.SessionId);
                                }
                                catch { }

                                if (customerSession.IsOnline)
                                {
                                    await Clients.Client(customerSession.ConnectionId).SendAsync("ChatClosed", new { sessionId = customerSession.SessionId });
                                }

                                customerSession.AssignedAdminId = null;
                            }
                        }

                        await Clients.Group("Admins").SendAsync("AdminLeft", new
                        {
                            AdminId = adminSession.AdminId,
                            AdminName = adminSession.AdminName
                        });

                        await BroadcastCustomerListUpdate();
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Cashier")]
        public async Task CloseSession(string sessionId)
        {
            // Đóng trạng thái phiên chat trong database
            await _chatService.CloseSessionAsync(sessionId);

            // Tìm user đang thuộc phiên này
            var userSession = _userSessions.Values.FirstOrDefault(u => u.SessionId == sessionId);

            if (userSession != null && userSession.IsOnline)
            {
                await Clients.Client(userSession.ConnectionId).SendAsync("ChatClosed", new { sessionId = sessionId });
            }
        }

        private string GetAdminId()
        {
            return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
           ?? throw new UnauthorizedAccessException("User không hợp lệ");
        }

        private string GetAdminName()
        {
            return Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? throw new UnauthorizedAccessException("User không hợp lệ");
        }

        private async Task BroadcastCustomerListUpdate()
        {
            var customerList = _userSessions.Values
                .Where(u => u.IsOnline)
                .Select(u => new
                {
                    u.UserId,
                    u.UserName,
                    u.StartTime,
                    u.AssignedAdminId,
                    IsAssigned = !string.IsNullOrEmpty(u.AssignedAdminId),
                    LastMessage = "",
                    LastMessageTime = u.StartTime,
                    UnreadCount = 0
                })
                .OrderByDescending(u => u.StartTime)
                .ToList();

            await Clients.Group("Admins").SendAsync("CustomerListUpdated", customerList);
        }
    }

    // Models
    public class UserSession
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
        public DateTime StartTime { get; set; }
        public bool IsOnline { get; set; }
        public string? AssignedAdminId { get; set; }
        public List<ChatMessage> WaitingMessages { get; set; } = new();
    }

    public class AdminSession
    {
        public string AdminId { get; set; }
        public string AdminName { get; set; }
        // multiple connections allowed for same adminId
        public HashSet<string> ConnectionIds { get; set; } = new();
        public List<string> AssignedUsers { get; set; } = new();
    }
}
