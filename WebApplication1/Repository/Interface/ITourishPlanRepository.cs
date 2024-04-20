using WebApplication1.Data;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface
{
    public interface ITourishPlanRepository
    {
        Response GetAll(string? search, string? category, string? startingPoint, string? endPoint, string? startingDate, double? priceFrom, double? priceTo, string? sortBy, int page = 1, int pageSize = 5);
        Response getById(Guid id);
        Response getByName(String TourName);
        Task<List<TourishInterest>> getTourInterest(Guid id);
        Task<Response> Add(TourishPlanInsertModel entityModel, String id);
        Task<Response> Update(TourishPlanUpdateModel entityModel, String id);
        Task<string> getDescription(string containerName, string blobName);
        Response Delete(Guid id);
    }
}
