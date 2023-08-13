using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;
namespace WebApplication1.Repository.InheritanceRepo;
public class BookRepository : IBookRepository
{
    private readonly MyDbContext _context;
    public static int PAGE_SIZE { get; set; } = 5;
    public BookRepository(MyDbContext _context)
    {
        this._context = _context;
    }

    public Response Add(BookModel bookModel)
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
        };
        _context.Add(book);
        _context.SaveChanges();

        return new Response
        {
            resultCd = 0,
            MessageCode = "I101",
            // Create type success               
        };

    }

    public Response Delete(Guid id)
    {
        var book = _context.Books.FirstOrDefault((book
          => book.id == id));
        if (book != null)
        {
            _context.Remove(book);
            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I103",
            // Delete type success               
        };
    }

    public Response GetAll(string? search, double? from, double? to, string? sortBy, int page = 1, int pageSize = 5)
    {
        var bookQuery = _context.Books.Include(book => book.BookStatus).
            Include(book => book.BookCategories).
            ThenInclude(book => book.Category).AsQueryable();

        #region Filtering
        if (!string.IsNullOrEmpty(search))
        {
            bookQuery = bookQuery.Where(book => book.Title.Contains(search));
        }
        if (from.HasValue)
        {
            bookQuery = bookQuery.Where(book => book.BookStatus.CurrentPrice >= from);
        }
        if (to.HasValue)
        {
            bookQuery = bookQuery.Where(book => book.BookStatus.CurrentPrice <= to);
        }
        #endregion

        #region Sorting
        //Default sort by Name (TenHh)
        bookQuery = bookQuery.OrderBy(book => book.Title);

        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy)
            {
                case "title_desc":
                    bookQuery = bookQuery.OrderByDescending(book => book.Title);
                    break;
                case "price_asc":
                    bookQuery = bookQuery.OrderBy(book => book.BookStatus.CurrentPrice);
                    break;
                case "price_desc":
                    bookQuery = bookQuery.OrderByDescending(book => book.BookStatus.CurrentPrice);
                    break;
                case "best_seller":
                    bookQuery = bookQuery.OrderByDescending(book => book.BookStatus.SoldNumberInMonth);
                    break;
            }
        }
        #endregion

        #region Paging
        var result = PaginatorModel<Book>.Create(bookQuery, page, PAGE_SIZE);
        #endregion

        var bookVM = new Response
        {
            resultCd = 0,
            Data = result.ToList(),
            count = result.TotalCount,
        };

        return bookVM;
    }

    public Response getById(Guid id)
    {
        var book = _context.Books.Where(book => book.id == id).Include(book => book.BookStatus).
           Include(book => book.BookCategories).
           ThenInclude(book => book.Category).FirstOrDefault();
        if (book == null) { return null; }

        return new Response
        {
            resultCd = 0,
            Data = book
        };
    }

    public Response getByName(String Title)
    {
        var book = _context.Books.FirstOrDefault((book
            => book.Title == Title));

        return new Response
        {
            resultCd = 0,
            Data = book
        };
    }

    public Response Update(BookModel bookModel)
    {
        var book = _context.Books.FirstOrDefault((book
            => book.id == bookModel.id));
        if (book != null)
        {
            book.UpdateDate = DateTime.UtcNow;
            book.Title = book.Title;
            book.Description = book.Description;
            book.PageNumber = book.PageNumber;
            book.CreateDate = book.CreateDate;
            book.PublisherId = book.PublisherId;
            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I102",
            // Update type success               
        };
    }
}
