using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface
{
    public interface IBookRepository
    {
        BookVM GetAll(string? search, double? from, double? to, string? sortBy, int page = 1);
        BookVM getById(Guid id);
        BookModel Add(BookModel bookModel);
        void Update(BookModel bookModel);
        void Delete(Guid id);
    }
}
