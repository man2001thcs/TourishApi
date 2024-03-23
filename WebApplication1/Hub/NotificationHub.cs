using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SignalR.Hub.Client;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Data.Connection;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;

namespace SignalR.Hub
{
    [Authorize]
    public class NotificationHub : Hub<INotificationHubClient>
    {
        private readonly MyDbContext _context;

        public NotificationHub(MyDbContext context, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _context = context;
        }

        public async Task SendOffersToAll(NotificationModel notification)
        {
            await Clients.All.SendOffersToAll(notification);
        }

        public async Task SendTotalNumber(Guid userId)
        {
            var notificationCount = _context.Notifications.Where(u => (u.UserReceiveId == userId || u.UserReceiveId == null) && !u.IsRead && !u.IsDeleted).Count();
            await Clients.All.SendString(notificationCount.ToString());
        }

        public async Task SendString(String a)
        {
            await Clients.All.SendString(a);
        }

        public async Task SendOffersToUser(Guid userReceiveId, NotificationModel notification)
        {
            try
            {
                var userSendId = Context.User.FindFirstValue("Id");

                if (userSendId != null && userSendId.Length > 0)
                {
                    if (notification.Id == null)
                    {
                        var notificationEntity = new Notification
                        {
                            UserCreateId = new Guid(userSendId),
                            UserReceiveId = userReceiveId,
                            Content = notification.Content,
                            IsRead = false,
                            IsDeleted = false,
                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow,

                        };
                        _context.Add(notificationEntity);
                    }



                    var connection = _context.NotificationConList.OrderByDescending(connection => connection.CreateDate).FirstOrDefault(u => u.UserId == userReceiveId && u.Connected);
                    if (connection != null)
                    {
                        await Clients.Client(connection.ConnectionID).SendOffersToUser(userReceiveId, notification);
                    }

                }
            }
            catch (Exception ex)
            {
                var userSendId = Context.User.FindFirstValue("Id");
                var connection = _context.NotificationConList.OrderByDescending(connection => connection.CreateDate).FirstOrDefault(u => u.UserId.ToString() == userSendId && u.Connected);
                //await Clients.Client(connection.ConnectionID).SendOffersToUser(userId, null);
                if (connection != null)
                {
                    await Clients.Client(connection.ConnectionID).SendError(new Guid(userSendId), "Lỗi xảy ra: " + ex.ToString());
                }
            }
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirstValue("Id");

            var user = _context.Users.Include(u => u.NotificationConList)
                .SingleOrDefault(u => u.Id.ToString() == userId);

            if (user != null)
            {

                var notifyCon = new NotificationCon
                {
                    UserId = user.Id,
                    ConnectionID = Context.ConnectionId,
                    UserAgent = Context.GetHttpContext().Request.Headers["User-Agent"].ToString(),
                    Connected = true,
                    CreateDate = DateTime.UtcNow
                };

                _context.Add(notifyCon);


                await _context.SaveChangesAsync();
            }


            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {

            var connection = _context.NotificationConList.FirstOrDefault(u => u.ConnectionID == Context.ConnectionId);
            if (connection != null)
                connection.Connected = false;
            await _context.SaveChangesAsync();

            await base.OnDisconnectedAsync(exception);
        }
    }
}