
using WebApplication1.Model;

namespace SignalR.Hub.Client
{
    public interface IGuestMessageHubClient
    {
        Task SendMessageToAdmin(Guid userId, GuestMessageModel message);
        Task SendMessageToGuest(String email, GuestMessageModel message);
        Task SendMessageToAll(GuestMessageModel message);
        Task SendString(String stringA);
        Task SendAdminError(Guid userId, string error);

        Task SendGuestError(String email, string error);
    }
}