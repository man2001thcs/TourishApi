﻿using System.Security.Claims;
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
using WebApplication1.Service.InheritanceService;

namespace SignalR.Hub
{
    public class GuestMessageHub : Hub<IGuestMessageHubClient>
    {
        private readonly MyDbContext _context;
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;

        public GuestMessageHub(
            MyDbContext context,
            IOptionsMonitor<AppSetting> optionsMonitor,
            NotificationService notificationService,
            UserService userService
        )
        {
            _context = context;
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task SendMessageToAll(GuestMessageModel message)
        {
            await Clients.All.SendMessageToAll(message);
        }

        public async Task SendMessageToUser(Guid adminId, string email, GuestMessageModel message)
        {
            try
            {
                var conHis = _context
                    .GuestMessageConHisList.Include(u => u.GuestCon)
                    .Include(u => u.AdminCon)
                    .ThenInclude(u => u.Admin)
                    .OrderByDescending(connection => connection.CreateDate)
                    .Where(u =>
                        u.GuestCon.GuestEmail == email
                        && u.GuestCon.Connected
                        && u.AdminCon.Connected
                        && u.AdminCon.Admin.Id == adminId
                    )
                    .FirstOrDefault();
                if (conHis != null)
                {
                    var messageEntity = new GuestMessage
                    {
                        Content = message.Content,
                        IsRead = false,
                        IsDeleted = false,
                        AdminMessageConId = conHis.AdminConId,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                    };

                    await _context.AddAsync(messageEntity);
                    await _context.SaveChangesAsync();

                    var connection = conHis.GuestCon;
                    if (connection != null)
                    {
                        var returnMess = message;
                        returnMess.State = 1;
                        returnMess.Id = messageEntity.Id;
                        returnMess.CreateDate = DateTime.UtcNow;
                        returnMess.Side = 1;
                        await Clients
                            .Client(Context.ConnectionId)
                            .SendMessageToAdmin(adminId, email, returnMess);
                        returnMess.State = 2;
                        returnMess.Side = 2;
                        await Clients
                            .Client(connection.ConnectionID)
                            .SendMessageToUser(adminId, email, returnMess);
                    }
                }
                else
                {

                    var returnMess = new GuestMessageModel();
                    returnMess.Content = "Kết nối đã đóng bởi khách hàng";
                    returnMess.CreateDate = DateTime.UtcNow;
                    returnMess.State = 2;
                    returnMess.Side = 2;
                    returnMess.IsClosed = true;
                    await Clients
                                        .Client(Context.ConnectionId)
                                        .SendMessageToAdmin(adminId, email, returnMess);

                }
            }
            catch (Exception ex)
            {
                var connectionAdmin = _context
                    .AdminMessageConList.OrderByDescending(connection => connection.CreateDate)
                    .FirstOrDefault(u => u.AdminId == adminId && u.Connected);

                if (connectionAdmin != null)
                {
                    var returnMess = message;
                    returnMess.State = 3;
                    await Clients
                        .Client(Context.ConnectionId)
                        .SendMessageToUser(adminId, email, returnMess);
                }
                //await Clients.Client(connection.ConnectionID).SendOffersToUser(userId, null);
            }
        }

        public async Task SendMessageToAdmin(Guid adminId, String email, GuestMessageModel message)
        {
            try
            {
                var conHis = _context
                    .GuestMessageConHisList.Include(u => u.GuestCon)
                    .Include(u => u.AdminCon)
                    .ThenInclude(u => u.Admin)
                    .OrderByDescending(connection => connection.CreateDate)
                    .Where(u =>
                        u.GuestCon.GuestEmail == email
                        && u.GuestCon.Connected
                        && u.AdminCon.Connected
                        && u.AdminCon.Admin.Id == adminId
                    )
                    .FirstOrDefault();

                if (conHis != null)
                {
                    var messageEntity = new GuestMessage
                    {
                        Content = message.Content,
                        IsRead = false,
                        IsDeleted = false,
                        GuestMessageConId = conHis.GuestConId,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                    };

                    await _context.AddAsync(messageEntity);
                    await _context.SaveChangesAsync();

                    var connection = conHis.AdminCon;
                    if (connection != null)
                    {
                        var returnMess = message;
                        returnMess.State = 1;
                        returnMess.Id = messageEntity.Id;
                        returnMess.CreateDate = DateTime.UtcNow;
                        returnMess.Side = 1;
                        await Clients
                            .Client(Context.ConnectionId)
                            .SendMessageToUser(adminId, email, returnMess);

                        returnMess.State = 2;
                        returnMess.Side = 2;
                        await Clients
                            .Client(connection.ConnectionID)
                            .SendMessageToAdmin(adminId, email, returnMess);
                    }
                }
            }
            catch (Exception ex)
            {
                var connectionGuest = _context
                    .GuestMessageConList.OrderByDescending(connection => connection.CreateDate)
                    .FirstOrDefault(u => u.GuestEmail == email && u.Connected);

                if (connectionGuest != null)
                {
                    var returnMess = message;
                    returnMess.State = 3;
                    await Clients
                        .Client(Context.ConnectionId)
                        .SendMessageToUser(adminId, email, returnMess);
                }
                //await Clients.Client(connection.ConnectionID).SendOffersToUser(userId, null);
            }
        }

        public override async Task OnConnectedAsync()
        {
            var adminId = (string)Context.GetHttpContext().Request.Query["adminId"];
            var guestEmail = (string)Context.GetHttpContext().Request.Query["guestEmail"];
            var guestName = (string)Context.GetHttpContext().Request.Query["guestName"];
            var guestPhoneNumber = (string)
                Context.GetHttpContext().Request.Query["guestPhoneNumber"];
            var token = Context.GetHttpContext().Request.Query["token"];

            if (!adminId.IsNullOrEmpty())
            {
                var tokenClaim = (ClaimsPrincipal)_userService.checkIfTokenFormIsValid(token).Data;
                if (tokenClaim == null)
                {
                    Context.Abort();
                }

                var userId = tokenClaim.FindFirstValue("Id");
                var userRole = tokenClaim.FindFirstValue("Role") ?? "";

                if (userRole != "Admin" && userRole != "AdminManager")
                {
                    Context.Abort();
                }
                else
                {
                    var admin = _context
                        .Users.Include(u => u.NotificationConList)
                        .SingleOrDefault(u => u.Id.ToString() == adminId);

                    if (admin != null)
                    {
                        var adminCon = new AdminMessageCon
                        {
                            AdminId = new Guid(adminId),
                            ConnectionID = Context.ConnectionId,
                            UserAgent = Context
                                .GetHttpContext()
                                .Request.Headers["User-Agent"]
                                .ToString(),
                            Connected = true,
                            CreateDate = DateTime.UtcNow
                        };

                        await _context.AddAsync(adminCon);

                        var conHis = _context
                            .GuestMessageConHisList.Include(u => u.GuestCon).Include(u => u.AdminCon)
                            .OrderByDescending(connection => connection.CreateDate)
                            .FirstOrDefault(u =>
                                u.GuestCon.GuestEmail == guestEmail
                                && u.GuestCon.GuestPhoneNumber == guestPhoneNumber
                                && u.GuestCon.Connected
                            );

                        if (conHis.AdminCon != null)
                        {
                            var oldMessageList = _context
                                .GuestMessages.Include(entity => entity.AdminMessageCon)
                                .Where(entity =>
                                    entity.AdminMessageConId
                                    == conHis.AdminConId
                                )
                                .ToList();

                            foreach (var message in oldMessageList)
                            {
                                message.AdminMessageConId = adminCon.Id;
                            }

                             await _context.SaveChangesAsync();

                            conHis.AdminCon = adminCon;
                            var adminInfo = new AdminMessageConDTOModel
                            {
                                AdminId = adminCon.AdminId,
                                ConnectionID = Context.ConnectionId,
                                AdminFullName = admin.FullName,
                                Connected = true
                            };
                            await Clients
                                .Client(conHis.GuestCon.ConnectionID)
                                .NotifyNewCon(adminId, adminInfo);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            conHis.AdminCon = adminCon;
                            var adminInfo = new AdminMessageConDTOModel
                            {
                                AdminId = adminCon.AdminId,
                                ConnectionID = Context.ConnectionId,
                                AdminFullName = admin.FullName,
                                Connected = true
                            };
                            await Clients
                                .Client(conHis.GuestCon.ConnectionID)
                                .NotifyNewCon(adminId, adminInfo);
                            await _context.SaveChangesAsync();
                        }
                    }
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

                var adminConList = await _context
                    .NotificationConList.Where(entity =>
                        entity.User.Role == UserRole.Admin && entity.Connected
                    )
                    .ToListAsync();
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
            var connectionAdmin = _context
                .AdminMessageConList.Include(entity => entity.GuestMessageConHis)
                .OrderByDescending(entity => entity.CreateDate)
                .FirstOrDefault(u => u.ConnectionID == Context.ConnectionId);
            if (connectionAdmin != null)
            {
                connectionAdmin.Connected = false;
                connectionAdmin.GuestMessageConHis.CloseDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            var guestConnection = _context
                .GuestMessageConList.Include(entity => entity.GuestMessageConHis).ThenInclude(entity => entity.AdminCon)
                .OrderByDescending(entity => entity.CreateDate)
                .FirstOrDefault(u => u.ConnectionID == Context.ConnectionId);
            if (guestConnection != null)
            {
                guestConnection.Connected = false;
                guestConnection.GuestMessageConHis.CloseDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var adminConnection = _context
                .AdminMessageConList.Include(entity => entity.GuestMessageConHis).ThenInclude(entity => entity.GuestCon).Include(entity => entity.Admin)
                .OrderByDescending(entity => entity.CreateDate)
                .FirstOrDefault(u => u.Connected && u.GuestMessageConHis.GuestConId == guestConnection.Id);

                if (adminConnection != null)
                {
                    var returnMess = new GuestMessageModel();
                    returnMess.Content = "Kết nối đã đóng bởi khách hàng";
                    returnMess.CreateDate = DateTime.UtcNow;
                    returnMess.State = 2;
                    returnMess.Side = 2;
                    returnMess.IsClosed = true;
                    
                    await Clients
                                        .Client(adminConnection.ConnectionID)
                                        .SendMessageToAdmin(adminConnection.Admin.Id, adminConnection.Admin.Email, returnMess);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
