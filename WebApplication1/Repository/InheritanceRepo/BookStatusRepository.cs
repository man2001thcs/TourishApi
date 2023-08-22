using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;
namespace WebApplication1.Repository.InheritanceRepo;
public class BookStatusRepository : IBookStatusRepository
{
    private readonly MyDbContext _context;
    public static int PAGE_SIZE { get; set; } = 5;
    public BookStatusRepository(MyDbContext _context)
    {
        this._context = _context;
    }

    public Response Add(BookStatusModel bookStatusModel)
    {

        var bookStatus = new BookStatus
        {
            ProductId = bookStatusModel.ProductId,
            RemainNumber = bookStatusModel.RemainNumber,
            SoldNumberInMonth = bookStatusModel.SoldNumberInMonth,
            TotalSoldNumber = bookStatusModel.TotalSoldNumber,
            CurrentPrice = bookStatusModel.CurrentPrice,
            UpdateDate = DateTime.UtcNow
        };
        _context.Add(bookStatus);
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
        var bookStatus = _context.BookStatusList.FirstOrDefault((bookStatus
          => bookStatus.ProductId == id));
        if (bookStatus != null)
        {
            _context.Remove(bookStatus);
            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I103",
            // Delete type success               
        };
    }

    public Response getById(Guid id)
    {
        var bookStatus = _context.BookStatusList.Where(bookStatus => bookStatus.ProductId == id).FirstOrDefault();
        if (bookStatus == null) { return null; }

        return new Response
        {
            resultCd = 0,
            Data = bookStatus
        };
    }

    public Response Update(BookStatusModel bookStatusModel)
    {
        var bookStatus = _context.BookStatusList.FirstOrDefault((bookStatus
            => bookStatus.ProductId == bookStatusModel.ProductId));
        if (bookStatus != null)
        {
            bookStatus.RemainNumber = bookStatusModel.RemainNumber;
            bookStatus.SoldNumberInMonth = bookStatusModel.SoldNumberInMonth;
            bookStatus.TotalSoldNumber = bookStatusModel.TotalSoldNumber;
            bookStatus.CurrentPrice = bookStatusModel.CurrentPrice;
            bookStatus.UpdateDate = DateTime.UtcNow;

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
