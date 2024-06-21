namespace TourishApi.Task;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.Receipt;
using WebApplication1.Data.Schedule;

public class ReceiptStatusChangeTask
{
    private readonly MyDbContext _context;

    public ReceiptStatusChangeTask(MyDbContext _context)
    {
        this._context = _context;
    }

    public async Task ReceiptStatusTask()
    {
        var onWaitTourReceipt = _context
            .FullReceiptList.Where(entity =>
                entity.CreatedDate < DateTime.UtcNow
                && (int)entity.Status < 2
                && (DateTime.UtcNow - entity.CreatedDate).TotalDays > 7
            )
            .OrderBy(entity => entity.CreatedDate)
            .AsSplitQuery()
            .ToList();

        foreach (var item in onWaitTourReceipt)
        {
            item.Status = FullReceiptStatus.Cancelled;
            item.CompleteDate = DateTime.UtcNow;
        }

        var onWaitServiceReceipt = _context
            .FullScheduleReceiptList.Where(entity =>
                entity.CreatedDate < DateTime.UtcNow
                && (int)entity.Status < 2
                && (DateTime.UtcNow - entity.CreatedDate).TotalDays > 7
            )
            .OrderBy(entity => entity.CreatedDate)
            .AsSplitQuery()
            .ToList();

        foreach (var item in onWaitServiceReceipt)
        {
            item.Status = FullReceiptStatus.Cancelled;
            item.CompleteDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }
}
