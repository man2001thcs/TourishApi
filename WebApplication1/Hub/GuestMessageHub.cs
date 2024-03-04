﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SignalR.Hub.Client;
using System.Security.Claims;
using TourishApi.Data.Chat;
using WebApplication1.Data;
using WebApplication1.Data.Connection;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;

namespace SignalR.Hub
{
    [Authorize]
    public class GuestMessageHub : Hub<IMessageHubClient>
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

        public async Task ReceiveTotalNumber(Guid userId)
        {
            var messageCount = _context.Messages.Where(u => u.UserReceiveId == userId && !u.IsRead && !u.IsDeleted).Count();
            await Clients.All.SendString(messageCount.ToString());
        }

        public async Task SendString(String a)
        {
            await Clients.All.SendString(a);
        }

        public async Task SendMessageToUser(Guid userSentId, Guid userReceiveId, GuestMessageModel message)
        {
            try
            {
                var fileSave = new List<SaveFile>();

                foreach (var file in message.Files)
                {
                    fileSave.Add(new SaveFile
                    {
                        FileType = file.FileType,
                        ResourceType = ResourceTypeEnum.Message,
                        CreatedDate = DateTime.UtcNow,
                    });
                }

                var messageEntity = new UserMessage
                {
                    UserSentId = userSentId,
                    UserReceiveId = userReceiveId,
                    Content = message.Content,
                    Files = fileSave,
                    IsRead = false,
                    IsDeleted = false,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,

                };
                _context.Add(messageEntity);


                var connection = _context.MessageConList
                    .OrderByDescending(connection => connection.CreateDate)
                    .FirstOrDefault(u => u.UserId == userReceiveId && u.Connected);
                if (connection != null)
                {
                    await Clients.Client(connection.ConnectionID).SendMessageToUser(userSentId, message);
                    await Clients.Client(connection.ConnectionID).SendMessageToUser(userReceiveId, message);
                }
            }
            catch (Exception ex)
            {
                var connection = _context.MessageConList
                    .OrderByDescending(connection => connection.CreateDate)
                    .FirstOrDefault(u => u.UserId == userSentId && u.Connected);

                if (connection != null)
                {
                    await Clients.Client(connection.ConnectionID).SendError(userSentId, "Lỗi xảy ra: " + ex.ToString());
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
            var userId = Context.User.FindFirstValue("Id");

            var user = _context.Users.Include(u => u.MessageConList)
                .SingleOrDefault(u => u.Id.ToString() == userId);

            if (user != null)
            {
                var notifyCon = new UserMessageCon
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

            var connection = _context.MessageConList.FirstOrDefault(u => u.ConnectionID == Context.ConnectionId);
            if (connection != null)
                connection.Connected = false;
            await _context.SaveChangesAsync();

            await base.OnDisconnectedAsync(exception);
        }
    }
}