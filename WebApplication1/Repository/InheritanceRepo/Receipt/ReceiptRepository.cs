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

    public async Task<Response> Add(FullReceiptInsertModel receiptModel)
    {
        var totalReceipt = _context.TotalReceiptList.FirstOrDefault(entity => entity.TourishPlanId == receiptModel.TourishPlanId);

        if (totalReceipt == null)
        {
            totalReceipt = new TotalReceipt
            {
                TourishPlanId = receiptModel.TourishPlanId,
                Status = ReceiptStatus.Created,
                Description = receiptModel.Description,

                CreatedDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            await _context.AddAsync(totalReceipt);
            await _context.SaveChangesAsync();
        }


        var fullReceipt = new FullReceipt
        {
            TotalReceiptId = totalReceipt.TotalReceiptId,
            OriginalPrice = receiptModel.OriginalPrice,
            GuestName = receiptModel.GuestName,
            Description = receiptModel.Description,
            PhoneNumber = receiptModel.PhoneNumber,
            Email = receiptModel.Email,
            TotalTicket = receiptModel.TotalTicket,
            DiscountAmount = receiptModel.DiscountAmount,
            DiscountFloat = receiptModel.DiscountFloat,
            Status = FullReceiptStatus.Created,
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow
        };

        var planExist = _context.TourishPlan.FirstOrDefault((plan
              => plan.Id == totalReceipt.TourishPlanId));

        if (planExist != null && planExist.RemainTicket >= receiptModel.TotalTicket)
        {
            planExist.RemainTicket = planExist.RemainTicket - receiptModel.TotalTicket;
            _context.Add(fullReceipt);

            await _context.SaveChangesAsync();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I511",
                returnId = fullReceipt.FullReceiptId,
                // Create type success               
            };
        }
        else
        {
            return new Response
            {
                resultCd = 0,
                MessageCode = "C515",
                returnId = fullReceipt.FullReceiptId,
                // Out of ticket              
            };
        }



    }

    public Response Delete(Guid id)
    {
        var receipt = _context.FullReceiptList.Include(entity => entity.TotalReceipt).FirstOrDefault((receipt
          => receipt.FullReceiptId == id));
        if (receipt != null)
        {
            var plan = _context.TourishPlan.FirstOrDefault((plan
                => plan.Id == receipt.TotalReceipt.TourishPlanId));
            if (plan != null)
            {
                plan.RemainTicket = plan.RemainTicket + receipt.TotalTicket;
            }
            _context.Remove(receipt);
            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I513",
            // Delete type success               
        };
    }

    public Response DeleteAll(Guid tourishPlanId)
    {
        var receipt = _context.TotalReceiptList.FirstOrDefault((receipt
          => receipt.TourishPlanId == tourishPlanId));
        if (receipt != null)
        {
            _context.Remove(receipt);

            var plan = _context.TourishPlan.FirstOrDefault((plan
               => plan.Id == tourishPlanId));
            if (plan != null)
            {
                plan.RemainTicket = plan.TotalTicket;
            }

            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I513",
            // Delete type success               
        };
    }

    public Response GetAll(string? tourishPlanId, ReceiptStatus? status, string? sortBy, int page = 1, int pageSize = 5)
    {
        var receiptQuery = _context.TotalReceiptList.Include(receipt => receipt.FullReceiptList)
            .Include(receipt => receipt.TourishPlan)
            .Include(receipt => receipt.TourishPlan.MovingSchedules)
            .Include(receipt => receipt.TourishPlan.EatSchedules)
            .Include(receipt => receipt.TourishPlan.StayingSchedules)
            .AsQueryable();

        #region Filtering
        if (!string.IsNullOrEmpty(tourishPlanId))
        {
            receiptQuery = receiptQuery.Where(receipt => receipt.TourishPlanId == new Guid(tourishPlanId));
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
        var receipt = _context.TotalReceiptList.Where(receipt => receipt.TotalReceiptId == id).Include(receipt => receipt.FullReceiptList)
           .FirstOrDefault();
        if (receipt == null) { return null; }

        return new Response
        {
            resultCd = 0,
            Data = receipt
        };
    }

    public Response getFullReceiptById(Guid id)
    {
        var receipt = _context.FullReceiptList.Where(receipt => receipt.FullReceiptId == id).Include(entity => entity.TotalReceipt).ThenInclude(entity => entity.TourishPlan)
           .FirstOrDefault();
        if (receipt == null) { return null; }

        return new Response
        {
            resultCd = 0,
            Data = receipt
        };
    }

    public async Task<Response> Update(FullReceiptUpdateModel receiptModel)
    {
        var receipt = _context.FullReceiptList.Include(entity => entity.TotalReceipt).FirstOrDefault((receipt
            => receipt.FullReceiptId == receiptModel.FullReceiptId));
        if (receipt != null)
        {
            var oldTotalTicket = receipt.TotalTicket;

            receipt.GuestName = receiptModel.GuestName;
            receipt.Status = receiptModel.Status;
            receipt.DiscountAmount = receiptModel.DiscountAmount;
            receipt.DiscountFloat = receiptModel.DiscountFloat;
            receipt.OriginalPrice = receiptModel.OriginalPrice;
            receipt.TotalTicket = receiptModel.TotalTicket;
            receipt.Description = receiptModel.Description;
            receipt.Status = receiptModel.Status;

            receipt.UpdateDate = DateTime.UtcNow;
            if (receiptModel.Status == FullReceiptStatus.Completed) receipt.CompleteDate = DateTime.UtcNow;

            var planExist = _context.TourishPlan.FirstOrDefault((plan
              => plan.Id == receipt.TotalReceipt.TourishPlanId));

            if (planExist != null && planExist.RemainTicket + oldTotalTicket >= receiptModel.TotalTicket)
            {
                planExist.RemainTicket = planExist.RemainTicket + oldTotalTicket - receiptModel.TotalTicket;

                await _context.SaveChangesAsync();
            }
            else
            {
                return new Response
                {
                    resultCd = 0,
                    MessageCode = "C515",
                    returnId = receipt.TotalReceiptId,
                    // Out of ticket              
                };
            }

            var totalReceiptComplete = await _context.TotalReceiptList.Where(receipt => receipt.TotalReceiptId == receiptModel.TotalReceiptId).Include(entity => entity.FullReceiptList).FirstOrDefaultAsync();

            var totalCount = totalReceiptComplete.FullReceiptList.Count();

            var createCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt => fullReceipt.Status == FullReceiptStatus.Created);
            var onGoingCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt => fullReceipt.Status == FullReceiptStatus.AwaitPayment);
            var complteteCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt => fullReceipt.Status == FullReceiptStatus.Completed);
            var cancelCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt => fullReceipt.Status == FullReceiptStatus.Cancelled);

            if (totalCount == createCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Created;
            }
            else if (onGoingCount < totalCount && onGoingCount > 1)
            {
                totalReceiptComplete.Status = ReceiptStatus.OnGoing;
            }
            else if (complteteCount == totalCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Completed;
            }
            else if (cancelCount == totalCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Cancelled;
            }


            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I512",
            // Update type success               
        };
    }
}
