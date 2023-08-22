using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface
{
    public interface IBookStatusRepository
    {
        Response getById(Guid id);
        Response Add(BookStatusModel bookStatusModel);
        Response Update(BookStatusModel bookkStatusModel);
        Response Delete(Guid id);
    }
}
