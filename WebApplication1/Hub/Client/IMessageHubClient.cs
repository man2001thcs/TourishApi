
using WebApplication1.Model;

namespace SignalR.Hub.Client
{
    public interface IMessageHubClient
    {
        Task SendMessageToUser(Guid userId, MessageModel notification);
        Task SendMessageToAll(MessageModel notification);
        Task SendString(String stringA);
        Task SendError(Guid userId, string error);
    }
}