
using WebApplication1.Model;

namespace SignalR.Hub.Client
{
    public interface INotificationHubClient
    {
        Task SendOffersToUser(Guid userId, NotificationModel notification);
        Task SendOffersToAll(NotificationModel notification);
        Task SendString(String stringA);
    }
}