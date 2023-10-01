using WebApplication1.Model.Restaurant;
using WebApplication1.Model.VirtualModel;

namespace TourishApi.Repository.Interface.Restaurant
{
    public interface IRestaurantRepository
    {
        Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5);
        Response getById(Guid id);
        Response getByName(string name);
        Response Add(RestaurantModel addModel);
        Response Update(RestaurantModel updateModel);
        Response Delete(Guid id);
    }
}
