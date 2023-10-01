using WebApplication1.Model.RestHouse;
using WebApplication1.Model.VirtualModel;

namespace TourishApi.Repository.Interface.Resthouse
{
    public interface IHotelRepository
    {
        Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5);
        Response getById(Guid id);
        Response getByName(string name);
        Response Add(HotelModel addModel);
        Response Update(HotelModel updateModel);
        Response Delete(Guid id);
    }
}
