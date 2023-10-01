using WebApplication1.Model.Transport;
using WebApplication1.Model.VirtualModel;

namespace TourishApi.Repository.Interface.Transport
{
    public interface IPassengerCarRepository
    {
        Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5);
        Response getById(Guid id);
        Response getByName(string name);
        Response Add(PassengerCarModel addModel);
        Response Update(PassengerCarModel updateModel);
        Response Delete(Guid id);
    }
}
