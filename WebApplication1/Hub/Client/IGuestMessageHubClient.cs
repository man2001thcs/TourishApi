
using WebApplication1.Data.Connection;
using WebApplication1.Model;

namespace SignalR.Hub.Client
{
    public interface IGuestMessageHubClient
    {
        Task SendMessageToAdmin(GuestMessageModel message);
        Task SendMessageToGuest(GuestMessageModel message);
        Task SendMessageToAll(GuestMessageModel message);
        Task SendString(String stringA);
        Task NotifyNewCon(String adminId, GuestMessageConHistory guestMessageConHistory);
        Task SendAdminError(Guid userId, string error);

        Task SendGuestError(String email, string error);
    }
}