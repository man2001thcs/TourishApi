using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;

namespace TourishApi.Service.Interface
{
    public interface ITourishPlanService
    {
        public Task<Response> CreateNew(string userId, TourishPlanInsertModel entityModel);
        public Response GetAll(
            string? search,
            string? category,
            string? sortBy,
            int page = 1,
            int pageSize = 5
        );
        public Response GetById(Guid id);
        public Task<Response> UpdateEntityById(string userId, TourishPlanUpdateModel entityModel);
        public Response DeleteById(Guid id);
    }
}
