using WebApplication1.Model.Schedule;
using WebApplication1.Model.VirtualModel;

namespace TourishApi.Repository.Interface.Schedule
{
    public interface IStayingScheduleRepository
    {
        Response GetAll(Guid planId, string? search, string? sortBy, int page = 1, int pageSize = 5);
        Response getById(Guid id);
        Response getByName(string name);
        Response Add(MovingScheduleModel addModel);
        Response Update(MovingScheduleModel updateModel);
        Response Delete(Guid id);
    }
}
