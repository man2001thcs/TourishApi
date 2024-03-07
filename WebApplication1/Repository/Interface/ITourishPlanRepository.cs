using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface
{
    public interface ITourishPlanRepository
    {
        Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5);
        Response getById(Guid id);
        Response getByName(String TourName);
<<<<<<< HEAD
        Task<Response> Add(TourishPlanInsertModel entityModel, String id);
        Task<Response> Update(TourishPlanUpdateModel entityModel, String id);
=======
        Task<Response> Add(TourishPlanInsertModel entityModel, Guid id);
        Task<Response> Update(TourishPlanUpdateModel entityModel, Guid id);
>>>>>>> a3c0c39 (Add migration)
        Response Delete(Guid id);
    }
}
