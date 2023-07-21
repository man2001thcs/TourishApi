using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface
{
    public interface IBookRepository
    {
        List<BookVM> GetAll();
        BookVM getById(Guid id);
        BookVM Add(BookModel bookModel);
        void Update(BookVM bookVM);
        void Delete(Guid id);
    }
}
