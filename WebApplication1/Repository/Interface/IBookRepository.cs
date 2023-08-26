using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface
{
    public interface IBookRepository
    {
        Response GetAll(string? search, double? from, double? to, string? sortBy, int page = 1, int pageSize = 5);
        Response getById(Guid id);
        Response getByName(String Title);
        Task<Response> Add(BookInsertModel bookModel);
        Task<Response> Update(BookUpdateModel bookModel);
        Response Delete(Guid id);
    }
}
