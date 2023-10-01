using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.Receipt;
using WebApplication1.Model;
using WebApplication1.Model.Receipt;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface.Receipt;

namespace WebApplication1.Repository.InheritanceRepo.Receipt;
public class ReceiptRepository : IReceiptRepository
{
    private readonly MyDbContext _context;
    public static int PAGE_SIZE { get; set; } = 5;
    private readonly char[] delimiter = new char[] { ';' };
    public ReceiptRepository(MyDbContext _context)
    {
        this._context = _context;
    }

    public async Task<Response> Add(TotalReceiptInsertModel receiptModel)
    {

        var receipt = new TotalReceipt
        {
            UserId = receiptModel.UserId,
            Status = receiptModel.Status,
            Description = receiptModel.Description,
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,

            FullReceiptList = AddFullReceipt(receiptModel.SingleReceiptString),

        };

        _context.Add(receipt);
        await _context.SaveChangesAsync();

        return new Response
        {
            resultCd = 0,
            MessageCode = "I4101",
            returnId = receipt.ReceiptId,
            // Create type success               
        };

    }

    public Response Delete(Guid id)
    {
        var receipt = _context.TotalReceiptList.FirstOrDefault((receipt
          => receipt.ReceiptId == id));
        if (receipt != null)
        {
            _context.Remove(receipt);
            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I4103",
            // Delete type success               
        };
    }

    public Response GetAll(string? userId, ReceiptStatus? status, string? sortBy, int page = 1, int pageSize = 5)
    {
        var receiptQuery = _context.TotalReceiptList.Include(receipt => receipt.FullReceiptList).
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
        var result = PaginatorModel<TotalReceipt>.Create(receiptQuery, page, PAGE_SIZE);
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
        var receipt = _context.TotalReceiptList.Where(receipt => receipt.ReceiptId == id).Include(receipt => receipt.FullReceiptList)
           .FirstOrDefault();
        if (receipt == null) { return null; }

        return new Response
        {
            resultCd = 0,
            Data = receipt
        };
    }

    public async Task<Response> Update(TotalReceiptModel receiptModel)
    {
        var receipt = _context.TotalReceiptList.FirstOrDefault((receipt
            => receipt.ReceiptId == receiptModel.ReceiptId));
        if (receipt != null)
        {
            receipt.Description = receiptModel.Description;
            receipt.Status = receiptModel.Status;

            receipt.UpdateDate = DateTime.UtcNow;

            if (receiptModel.Status == ReceiptStatus.Completed) receipt.CompleteDate = DateTime.UtcNow;

            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I4102",
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
                    ServiceId = FullReceipt.ServiceId,
                    ServiceType = FullReceipt.ServiceType,
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
