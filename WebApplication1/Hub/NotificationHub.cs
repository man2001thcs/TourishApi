using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SignalR.Hub.Client;
using System.Diagnostics;
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
            var notificationCount = _context.Notifications.Where(u => u.UserId == userId && !u.IsRead && !u.IsDeleted).Count();
            await Clients.All.SendString(notificationCount.ToString());
        }

        public async Task SendString(String a)
        {
            await Clients.All.SendString(a);
        }

        public async Task SendOffersToUser(Guid userId, NotificationModel notification)
        {
            try
            {
                var notificationEntity = new Notification
                {
                    UserId = userId,
                    Content = notification.Content,
                    IsRead = false,
                    IsDeleted = false,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,

                };
                _context.Add(notificationEntity);


                var connection = _context.NotificationConList.FirstOrDefault(u => u.UserId == userId && u.Connected);
                if (connection != null)
                {
                    await Clients.Client(connection.ConnectionID).SendOffersToUser(userId, notification);
                }
                else
                {
                    await Clients.Client(connection.ConnectionID).SendOffersToUser(userId, null);
                }
            }
            catch (Exception ex)
            {
                var connection = _context.NotificationConList.FirstOrDefault(u => u.UserId == userId && u.Connected);
                //await Clients.Client(connection.ConnectionID).SendOffersToUser(userId, null);
                await Clients.Client(connection.ConnectionID).SendString("gg");
            }

        }

        //public Task SendMessageToGroup(Guid userId, string message)
        //{
        //    return Clients.Group("SignalR Users").SendAsync("ReceiveMessage", user, message);
        //}

        public override async Task OnConnectedAsync()
        {
            var name = Context.User.Identity.Name;
            var userId = Context.User.FindFirstValue("Id");

            Debug.WriteLine("Name: " + Context.User.FindFirstValue("Id"));

            var user = _context.Users.Include(u => u.NotificationConList)
                .SingleOrDefault(u => u.Id.ToString() == userId);

            if (user != null)
            {
                _context.NotificationConList.Where(u => u.UserId.ToString() == userId && u.Connected).ExecuteUpdate(f => f.SetProperty(x => x.Connected, false));

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
            Debug.WriteLine("Disconnected!!");

            await base.OnDisconnectedAsync(exception);
        }
    }
}