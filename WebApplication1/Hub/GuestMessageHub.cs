using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SignalR.Hub.Client;
using TourishApi.Service.InheritanceService;
using WebApplication1.Data;
using WebApplication1.Data.Chat;
using WebApplication1.Data.Connection;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.Connection;

namespace SignalR.Hub
{
    public class GuestMessageHub : Hub<IGuestMessageHubClient>
    {
        private readonly MyDbContext _context;
        private readonly NotificationService _notificationService;

        public GuestMessageHub(MyDbContext context, IOptionsMonitor<AppSetting> optionsMonitor, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
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
                var connectionAdmin = _context.AdminMessageConList
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

                var connection = _context.AdminMessageConList
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
            var adminId = (string)Context.GetHttpContext().Request.Query["adminId"];

            var guestEmail = (string)Context.GetHttpContext().Request.Query["guestEmail"];
            var guestName = (string)Context.GetHttpContext().Request.Query["guestName"];
            var guestPhoneNumber = (string)Context.GetHttpContext().Request.Query["guestPhoneNumber"];

            if (!adminId.IsNullOrEmpty())
            {
                var admin = _context.Users.Include(u => u.NotificationConList)
                    .SingleOrDefault(u => u.Id.ToString() == adminId);

                if (admin != null)
                {
                    var adminCon = new AdminMessageCon
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
                    var adminInfo = new AdminMessageConDTOModel
                    {
                        AdminId = adminCon.AdminId,
                        ConnectionID = Context.ConnectionId,
                        AdminFullName = admin.FullName,
                        Connected = true
                    };
                    await Clients.Client(conHis.GuestCon.ConnectionID).NotifyNewCon(conHis.GuestCon.GuestEmail, adminInfo);
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

                var adminConList = await _context.NotificationConList.Where(entity => entity.User.Role == UserRole.Admin && entity.Connected).ToListAsync();
                foreach (var adminCon in adminConList)
                {
                    var notification = new NotificationModel
                    {
                        UserCreateId = null,
                        UserReceiveId = adminCon.UserId,
                        TourishPlanId = null,
                        Content = "Hệ thống nhận được yêu cầu tư vấn mới",
                        ContentCode = "",
                        IsRead = false,
                        IsDeleted = false,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow
                    };

                    await _notificationService.CreateNewAsync(adminCon.UserId, notification);
                }


            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {

            var connectionAdmin = _context.AdminMessageConList.Include(entity => entity.GuestMessageConHis).OrderByDescending(entity => entity.CreateDate).FirstOrDefault(u => u.ConnectionID == Context.ConnectionId);
            if (connectionAdmin != null)
            {
                connectionAdmin.Connected = false;
                connectionAdmin.GuestMessageConHis.CloseDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

            }

            var guestConnection = _context.GuestMessageConList.Include(entity => entity.GuestMessageConHis).OrderByDescending(entity => entity.CreateDate).FirstOrDefault(u => u.ConnectionID == Context.ConnectionId);
            if (guestConnection != null)
            {
                guestConnection.Connected = false;
                guestConnection.GuestMessageConHis.CloseDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}