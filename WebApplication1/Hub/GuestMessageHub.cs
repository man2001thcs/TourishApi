using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SignalR.Hub.Client;
using WebApplication1.Data.Chat;
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

        [Authorize]
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
                await _context.AddAsync(messageEntity);
                await _context.SaveChangesAsync();


                var connection = _context.GuestMessageConList
                   .OrderByDescending(connection => connection.CreateDate)
                   .FirstOrDefault(u => u.GuestEmail == email && u.Connected);
                if (connection != null)
                {
                    await Clients.Client(connection.ConnectionID).SendMessageToGuest(message);
                    await Clients.Client(Context.ConnectionId).SendMessageToAdmin(message);
                }
            }
            catch (Exception ex)
            {
                var connectionAdmin = _context.GuestMessageConList
                    .OrderByDescending(connection => connection.CreateDate)
                    .FirstOrDefault(u => u.AdminId == adminId && u.Connected);

                if (connectionAdmin != null)
                {
                    await Clients.Client(connectionAdmin.ConnectionID).SendAdminError(adminId, "Lỗi xảy ra: " + ex.ToString());
                }
                //await Clients.Client(connection.ConnectionID).SendOffersToUser(userId, null);

            }
        }

        public async Task SendMessageToAdmin(Guid adminId, String email, GuestMessageModel message)
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

                await _context.AddAsync(messageEntity);
                await _context.SaveChangesAsync();

                var connection = _context.GuestMessageConList
                    .OrderByDescending(connection => connection.CreateDate)
                    .FirstOrDefault(u => u.AdminId == adminId && u.Connected);
                if (connection != null)
                {
                    await Clients.Client(connection.ConnectionID).SendMessageToAdmin(message);
                    await Clients.Client(Context.ConnectionId).SendMessageToGuest(message);
                }
            }
            catch (Exception ex)
            {
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

        public override async Task OnConnectedAsync()
        {
            var adminId = Context.GetHttpContext().Request.Query["adminId"];

            var guestEmail = Context.GetHttpContext().Request.Query["guestEmail"];
            var guestName = Context.GetHttpContext().Request.Query["guestName"];
            var guestPhoneNumber = Context.GetHttpContext().Request.Query["guestPhoneNumber"];

            if (!adminId.IsNullOrEmpty())
            {
                var admin = _context.Users.Include(u => u.NotificationConList)
                    .SingleOrDefault(u => u.Id.ToString() == adminId);

                if (admin != null)
                {
                    var adminCon = new GuestMessageCon
                    {
                        AdminId = new Guid(adminId),
                        ConnectionID = Context.ConnectionId,
                        UserAgent = Context.GetHttpContext().Request.Headers["User-Agent"].ToString(),
                        Connected = true,
                        CreateDate = DateTime.UtcNow
                    };

                    await _context.AddAsync(adminCon);

                    var conHis = _context.GuestMessageConHisList.Include(u => u.GuestCon).OrderByDescending(connection => connection.CreateDate)
                                        .SingleOrDefault(u => u.GuestCon.GuestEmail == guestEmail && u.GuestCon.GuestPhoneNumber == guestPhoneNumber && u.GuestCon.Connected);

                    conHis.AdminCon = adminCon;

                    await _context.SaveChangesAsync();
                }
            }
            else if (!guestEmail.IsNullOrEmpty())
            {
                var guestCon = new GuestMessageCon
                {
                    GuestEmail = guestEmail,
                    GuestName = guestName,
                    GuestPhoneNumber = guestPhoneNumber,
                    ConnectionID = Context.ConnectionId,
                    UserAgent = Context.GetHttpContext().Request.Headers["User-Agent"].ToString(),
                    Connected = true,
                    CreateDate = DateTime.UtcNow
                };

                var hisCon = new GuestMessageConHistory
                {
                    GuestCon = guestCon,
                    CreateDate = DateTime.UtcNow
                };

                await _context.AddAsync(hisCon);
                await _context.SaveChangesAsync();

                var adminConList = await _context.GuestMessageConList.Where(entity => entity.AdminId != null && entity.Connected).ToListAsync();
                foreach (var adminCon in adminConList)
                {
                    await Clients.Client(adminCon.ConnectionID).NotifyNewCon(adminId, hisCon);
                }
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