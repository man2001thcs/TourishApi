namespace TourishApi.Task;

using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.Schedule;

public class RemoveOldNotifyTask
{
    private readonly MyDbContext _context;

    public RemoveOldNotifyTask(MyDbContext _context)
    {
        this._context = _context;
    }

    public async Task RemoveOldNotify()
    {
        var notificationList = _context
            .Notifications.Where(entity =>
                (DateTime.UtcNow - (entity.CreateDate ?? DateTime.UtcNow)).TotalDays >= 7
            )
            .OrderBy(entity => entity.CreateDate)
            .AsSplitQuery()
            .ToList();

        foreach (var notification in notificationList)
        {
            _context.Remove(notification);
        }

        await _context.SaveChangesAsync();
    }
}