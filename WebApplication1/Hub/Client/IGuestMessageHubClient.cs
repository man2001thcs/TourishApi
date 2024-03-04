﻿
using WebApplication1.Model;

namespace SignalR.Hub.Client
{
    public interface IGuestMessageHubClient
    {
        Task SendMessageToUser(Guid userId, UserMessageModel notification);
        Task SendMessageToAll(UserMessageModel notification);
        Task SendString(String stringA);
        Task SendError(Guid userId, string error);
    }
}