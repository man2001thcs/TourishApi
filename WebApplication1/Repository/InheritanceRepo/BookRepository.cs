using WebApplication1.Data;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;
namespace WebApplication1.Repository.InheritanceRepo;
    public class BookRepository : IBookRepository
{
    private readonly MyDbContext _context;
    public BookRepository(MyDbContext _context)
    {
        this._context = _context;
    }

    public BookVM Add(BookModel bookModel)
    {

        var book = new Book
        {
            id = new Guid(),
            Title = bookModel.Title,
            Description = bookModel.Description,
            PageNumber = bookModel.PageNumber,
            CreateDate = bookModel.CreateDate,
            UpdateDate = bookModel.UpdateDate,
            PublisherId = bookModel.PublisherId,
            AuthorId = bookModel.AuthorId,
        };
        _context.Add(book);
        _context.SaveChanges();

        return new BookVM
        {
            id = bookModel.id,
            Title = bookModel.Title,
            Description = bookModel.Description,
            PageNumber = bookModel.PageNumber,
            CreateDate = bookModel.CreateDate,
            UpdateDate = bookModel.UpdateDate,
            PublisherId = bookModel.PublisherId,
            AuthorId = bookModel.AuthorId,
        };

    }

    public void Delete(Guid id)
    {
        var book = _context.Books.FirstOrDefault((book
           => book.id == id));
        if (book != null)
        {
            _context.Remove(book);
            _context.SaveChanges();
        }
    }

    public List<BookVM> GetAll()
    {
        var bookList = _context.Books.Select(book => new BookVM
        {
            id = book.id,
            Title = book.Title,
            Description = book.Description,
            PageNumber = book.PageNumber,
            CreateDate = book.CreateDate,
            UpdateDate = book.UpdateDate,
            PublisherId = book.PublisherId,
            AuthorId = book.AuthorId,
        });
        return bookList.ToList();

    }

    public BookVM getById(Guid id)
    {
        var book = _context.Books.FirstOrDefault((book
            => book.id == id));
        if (book == null) { return null; }
        return new BookVM
        {
            id = book.id,
            Title = book.Title,
            Description = book.Description,
            PageNumber = book.PageNumber,
            CreateDate = book.CreateDate,
            UpdateDate = book.UpdateDate,
            PublisherId = book.PublisherId,
            AuthorId = book.AuthorId,
        };
    }

    public void Update(BookVM bookVM)
    {
        var book = _context.Books.FirstOrDefault((book
            => book.id == bookVM.id));
        if (book != null)
        {
            book.UpdateDate = DateTime.UtcNow;
            book.Title = book.Title;
            book.Description = book.Description;
            book.PageNumber = book.PageNumber;
            book.CreateDate = book.CreateDate;
            book.PublisherId = book.PublisherId;
            book.AuthorId = book.AuthorId;
            _context.SaveChanges();
        }
    }
}
}
