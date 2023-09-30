using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;
namespace WebApplication1.Repository.InheritanceRepo;
public class ReceiptRepository : IReceiptRepository
{
    private readonly MyDbContext _context;
    public static int PAGE_SIZE { get; set; } = 5;
    private readonly char[] delimiter = new char[] { ';' };
    public ReceiptRepository(MyDbContext _context)
    {
        this._context = _context;
    }

    public async Task<Response> Add(ReceiptInsertModel receiptModel)
    {

        var receipt = new Receipt
        {
            UserId = receiptModel.UserId,
            Status = receiptModel.Status,
            Description = receiptModel.Description,
            TransportMethod = receiptModel.TransportMethod,
            VoucherId = receiptModel.VoucherId,
            CreatedDate = DateTime.UtcNow,
            CompleteDate = DateTime.UtcNow,

            FullReceiptList = AddFullReceipt(receiptModel.SingleReceiptString),

        };

        _context.Add(receipt);
        await _context.SaveChangesAsync();

        return new Response
        {
            resultCd = 0,
            MessageCode = "I801",
            returnId = receipt.ReceiptId,
            // Create type success               
        };

    }

    public Response Delete(Guid id)
    {
        var receipt = _context.ReceiptList.FirstOrDefault((receipt
          => receipt.ReceiptId == id));
        if (receipt != null)
        {
            _context.Remove(receipt);
            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I803",
            // Delete type success               
        };
    }

    public Response GetAll(string? userId, ReceiptStatus? status, string? sortBy, int page = 1, int pageSize = 5)
    {
        var receiptQuery = _context.ReceiptList.Include(receipt => receipt.FullReceiptList).
            AsQueryable();

        #region Filtering
        if (!string.IsNullOrEmpty(userId))
        {
            receiptQuery = receiptQuery.Where(receipt => receipt.UserId == new Guid(userId));
        }

        if (!string.IsNullOrEmpty(status.ToString()))
        {
            receiptQuery = receiptQuery.Where(receipt => receipt.Status == status);
        }
        #endregion

        #region Sorting
        //Default sort by Name (TenHh)
        receiptQuery = receiptQuery.OrderBy(receipt => receipt.UpdateDate);

        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy)
            {
                case "status_desc":
                    receiptQuery = receiptQuery.OrderByDescending(receipt => receipt.Status);
                    break;
            }
        }
        #endregion

        #region Paging
        var result = PaginatorModel<Receipt>.Create(receiptQuery, page, PAGE_SIZE);
        #endregion

        var receiptVM = new Response
        {
            resultCd = 0,
            Data = result.ToList(),
            count = result.TotalCount,
        };

        return receiptVM;
    }

    public Response getById(Guid id)
    {
        var receipt = _context.ReceiptList.Where(receipt => receipt.ReceiptId == id).Include(receipt => receipt.FullReceiptList)
           .FirstOrDefault();
        if (receipt == null) { return null; }

        return new Response
        {
            resultCd = 0,
            Data = receipt
        };
    }

    public async Task<Response> Update(ReceiptUpdateModel receiptModel)
    {
        var receipt = _context.ReceiptList.FirstOrDefault((receipt
            => receipt.ReceiptId == receiptModel.ReceiptId));
        if (receipt != null)
        {
            receipt.Description = receiptModel.Description;
            receipt.TransportMethod = receiptModel.TransportMethod ?? receipt.TransportMethod;
            receipt.Status = receiptModel.Status;

            receipt.UpdateDate = DateTime.UtcNow;

            if (receiptModel.Status == ReceiptStatus.Completed) receipt.CompleteDate = DateTime.UtcNow;

            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I802",
            // Update type success               
        };
    }

    private List<FullReceipt> AddFullReceipt(string FullReceiptString)
    {
        var receiptList = new List<FullReceipt>();

        dynamic FullReceiptArray = Newtonsoft.Json.JsonConvert.DeserializeObject(FullReceiptString);

        if (FullReceiptArray.Length > 0)
        {
            foreach (var FullReceipt in FullReceiptArray)
            {
                receiptList.Add(new FullReceipt
                {
                    ProductId = FullReceipt.ProductId,
                    TotalNumber = FullReceipt.TotalNumber,
                    SinglePrice = FullReceipt.SinglePrice,
                    DiscountAmount = FullReceipt.DiscountAmount,
                    DiscountFloat = FullReceipt.DiscountFloat,
                });
            }
        }
        return receiptList;
    }

}
