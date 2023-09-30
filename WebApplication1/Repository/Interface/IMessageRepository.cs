using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface
{
    public interface IMessageRepository
    {
        Response GetAll(string? search, Guid UserId, string? sortBy, int page = 1, int pageSize = 5);
        Response getById(Guid id);
        Response getByName(String name);
        Response Add(MessageModel notificationModel);
        Response Update(MessageModel notificationModel);
        Response Delete(Guid id);
    }
}
