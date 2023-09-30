using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.RelationData;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;
namespace WebApplication1.Repository.InheritanceRepo;
public class BookRepository : IBookRepository
{
    private readonly MyDbContext _context;
    public static int PAGE_SIZE { get; set; } = 5;
    private readonly char[] delimiter = new char[] { ';' };
    public BookRepository(MyDbContext _context)
    {
        this._context = _context;
    }

    public async Task<Response> Add(BookInsertModel bookModel)
    {

        var bookStatus = new BookStatus
        {
            CurrentPrice = bookModel.CurrentPrice,
            TotalSoldNumber = bookModel.TotalSoldNumber,
            RemainNumber = bookModel.RemainNumber,
            SoldNumberInMonth = bookModel.SoldNumberInMonth,
        };


        var book = new Book
        {
            Title = bookModel.Title,
            Description = bookModel.Description,
            PageNumber = bookModel.PageNumber,
            PublisherId = bookModel.PublisherId,

            BookSize = bookModel.BookSize,
            BookWeight = bookModel.BookWeight,
            CoverMaterial = bookModel.CoverMaterial,
            PublishYear = bookModel.PublishYear,
            Language = bookModel.Language,

            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,

            BookStatus = bookStatus,
            BookAuthors = AddBookAuthor(bookModel.AuthorRelationString),
            BookCategories = AddBookCategory(bookModel.CategoryRelationString),
            BookVouchers = AddBookVoucher(bookModel.VoucherRelationString)
        };

        _context.Add(book);
        await _context.SaveChangesAsync();

        return new Response
        {
            resultCd = 0,
            MessageCode = "I101",
            returnId = book.id,
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

    public Response GetAll(string? search, double? from, double? to, string? sortBy, string? category,
        string? publisherString, string? authorString, float? saleFloat, int page = 1, int pageSize = 5)
    {
        var bookQuery = _context.Books.Include(book => book.BookStatus).
            Include(book => book.BookCategories).
            ThenInclude(book => book.Category).
            Include(Book => Book.BookVouchers).
            ThenInclude(book => book.Voucher).
            AsQueryable();

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

        #region Category
        if (!string.IsNullOrEmpty(category))
        {
            bookQuery = bookQuery.Where(book =>
                book.BookCategories.Count(categoryOne =>
                    categoryOne.Category.Name.Contains(category)
                ) > 0
               );
        }
        #endregion

        #region SaleFloat
        if (saleFloat > 0)
        {
            bookQuery = bookQuery.Where(book =>
                book.BookVouchers.Count(voucher =>
                    voucher.Voucher.DiscountFloat >= saleFloat
                ) > 0
               );
        }
        #endregion

        #region Publisher
        if (!string.IsNullOrEmpty(publisherString))
        {
            string[] publisherArray = publisherString.Split(delimiter);

            if (publisherArray.Length > 0)
            {
                bookQuery = bookQuery.Where(book =>
                   publisherArray.Contains(book.Publisher.PublisherName)

                );
            }
        }
        #endregion


        #region Author
        if (!string.IsNullOrEmpty(authorString))
        {
            string[] authorArray = authorString.Split(delimiter);

            if (authorArray.Length > 0)
            {
                bookQuery = bookQuery.Where(book =>
                book.BookAuthors.Count(author =>
                    authorArray.Contains(author.Author.Name)) > 0
               );
            }
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
        var book = _context.Books.Where(book => book.id == id).Include(book => book.BookStatus)
            .Include(book => book.Publisher)
           .Include(book => book.BookCategories)
           .ThenInclude(book => book.Category)
            .Include(book => book.BookVouchers)
           .ThenInclude(book => book.Voucher)
            .Include(book => book.BookAuthors)
           .ThenInclude(book => book.Author)
           .FirstOrDefault();
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

    public async Task<Response> Update(BookUpdateModel bookModel)
    {
        var book = _context.Books.FirstOrDefault((book
            => book.id == bookModel.id));
        if (book != null)
        {

            book.Title = bookModel.Title ?? book.Title;
            book.Description = bookModel.Description ?? book.Description;
            book.PageNumber = bookModel.PageNumber ?? book.PageNumber;
            book.PublisherId = bookModel.PublisherId ?? book.PublisherId;
            book.Language = bookModel.Language;

            book.BookSize = bookModel.BookSize ?? book.BookSize;

            if (!float.IsNaN(bookModel.BookWeight))
            {
                book.BookWeight = bookModel.BookWeight;
            }


            book.CoverMaterial = bookModel.CoverMaterial;

            book.PublishYear = bookModel.PublishYear;


            if (bookModel.BookStatus != null)
            {
                book.BookStatus = bookModel.BookStatus;
            };


            if (bookModel.AuthorRelationString != "" && bookModel.AuthorRelationString != null)
            {
                await _context.BookAuthorList.Where(a => a.BookId == bookModel.id).ExecuteDeleteAsync();
                book.BookAuthors = AddBookAuthor(bookModel.AuthorRelationString);
            }

            if (bookModel.CategoryRelationString != "" && bookModel.CategoryRelationString != null)
            {
                await _context.BookCategoryList.Where(a => a.BookId == bookModel.id).ExecuteDeleteAsync();
                book.BookCategories = AddBookCategory(bookModel.CategoryRelationString);
            }

            if (bookModel.VoucherRelationString != "" && bookModel.VoucherRelationString != null)
            {
                await _context.BookVoucherList.Where(a => a.BookId == bookModel.id).ExecuteDeleteAsync();
                book.BookVouchers = AddBookVoucher(bookModel.VoucherRelationString);
            }

            book.UpdateDate = DateTime.UtcNow;

            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I102",
            // Update type success               
        };
    }

    private List<BookAuthor> AddBookAuthor(string RelationArrayString)
    {
        var bookAuthorList = new List<BookAuthor>();

        string[] RelationArray = RelationArrayString.Split(delimiter);

        if (RelationArray.Length > 0)
        {
            foreach (var Relation in RelationArray)
            {
                bookAuthorList.Add(new BookAuthor
                {
                    AuthorId = Guid.Parse(Relation)
                });
            }
        }
        return bookAuthorList;
    }

    private List<BookVoucher> AddBookVoucher(string RelationArrayString)
    {
        var bookVoucherList = new List<BookVoucher>();

        string[] RelationArray = RelationArrayString.Split(delimiter);

        if (RelationArray.Length > 0)
        {
            foreach (var Relation in RelationArray)
            {
                bookVoucherList.Add(new BookVoucher
                {
                    VoucherId = Guid.Parse(Relation)
                });
            }
        }
        return bookVoucherList;
    }

    private List<BookCategory> AddBookCategory(string RelationArrayString)
    {
        var bookCategoryList = new List<BookCategory>();

        string[] RelationArray = RelationArrayString.Split(delimiter);

        if (RelationArray.Length > 0)
        {
            foreach (var Relation in RelationArray)
            {
                bookCategoryList.Add(new BookCategory
                {
                    CategoryId = Guid.Parse(Relation)
                });
            }
        }
        return bookCategoryList;
    }
}
