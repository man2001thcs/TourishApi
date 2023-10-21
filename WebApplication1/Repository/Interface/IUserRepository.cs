using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface
{
    public interface IUserRepository
    {
        Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5);
        Response getById(Guid id);
        Response getByName(String userName);
        Task<Response> UpdateInfo(UserUpdateModel entityModel);
        Task<Response> UpdatePassword(UserUpdatePasswordModel entityModel);
        Response Delete(Guid id);
    }
}
