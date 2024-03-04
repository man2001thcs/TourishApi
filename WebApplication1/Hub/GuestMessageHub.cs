using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SignalR.Hub.Client;
using System.Security.Claims;
using WebApplication1.Data.Chat;
using WebApplication1.Data;
using WebApplication1.Data.Connection;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;

namespace SignalR.Hub
{
    public class GuestMessageHub : Hub<IGuestMessageHubClient>
    {
        private readonly MyDbContext _context;

        public GuestMessageHub(MyDbContext context, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _context = context;
        }

        public async Task SendMessageToAll(GuestMessageModel message)
        {
            await Clients.All.SendMessageToAll(message);
        }

        public async Task SendMessageToUser(Guid adminId, string email, GuestMessageModel message)
        {
            try
            {

                var messageEntity = new GuestMessage
                {        
                    Content = message.Content,
                    IsRead = false,
                    IsDeleted = false,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,

                };
                _context.Add(messageEntity);


                var connection = _context.GuestMessageConList
                    .OrderByDescending(connection => connection.CreateDate)
                    .FirstOrDefault(u => u.AdminId == adminId && u.GuestEmail == email && u.Connected);
                if (connection != null)
                {
                    await Clients.Client(connection.ConnectionID).SendMessageToAdmin(adminId, message);
                    await Clients.Client(connection.ConnectionID).SendMessageToGuest(email, message);
                }
            }
            catch (Exception ex)
            {
                var connectionAdmin = _context.GuestMessageConList
                    .OrderByDescending(connection => connection.CreateDate)
                    .FirstOrDefault(u => u.AdminId == adminId  && u.Connected);

                if (connectionAdmin != null)
                {
                    await Clients.Client(connectionAdmin.ConnectionID).SendAdminError(adminId, "Lỗi xảy ra: " + ex.ToString());
                }

                var connectionGuest = _context.GuestMessageConList
                   .OrderByDescending(connection => connection.CreateDate)
                   .FirstOrDefault(u => u.GuestEmail == email && u.Connected);

                if (connectionGuest != null)
                {
                    await Clients.Client(connectionGuest.ConnectionID).SendGuestError(email, "Lỗi xảy ra: " + ex.ToString());
                }
                //await Clients.Client(connection.ConnectionID).SendOffersToUser(userId, null);

            }

        }


        //public Task SendMessageToGroup(Guid userId, string message)
        //{
        //    return Clients.Group("SignalR Users").SendAsync("ReceiveMessage", user, message);
        //}

        public override async Task OnConnectedAsync()
        {
            var adminId = Context.GetHttpContext().Request.Query["adminId"];
            var guestEmail = Context.GetHttpContext().Request.Query["guestEmail"];
            var guestName = Context.GetHttpContext().Request.Query["guestName"];
            var guestPhoneNumber = Context.GetHttpContext().Request.Query["guestPhoneNumber"];

            var admin = _context.Users.Include(u => u.NotificationConList)
               .SingleOrDefault(u => u.Id.ToString() == adminId);

            if (admin != null)
            {
                var notifyCon = new GuestMessageCon
                {
                    AdminId = new Guid(adminId),
                    GuestEmail = guestEmail,
                    GuestName = guestName,
                    GuestPhoneNumber = guestPhoneNumber,
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

            var connection = _context.GuestMessageConList.FirstOrDefault(u => u.ConnectionID == Context.ConnectionId);
            if (connection != null)
                connection.Connected = false;
            await _context.SaveChangesAsync();

            await base.OnDisconnectedAsync(exception);
        }
    }
}