namespace TourishApi.Task;

using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.Schedule;

public class RemoveOldMessageTask
{
    private readonly MyDbContext _context;

    public RemoveOldMessageTask(MyDbContext _context)
    {
        this._context = _context;
    }

    public async Task RemoveOldMessage()
    {
        var guestMessageList = _context
            .GuestMessages
            .Where(entity => (DateTime.UtcNow - (entity.CreateDate ?? DateTime.UtcNow)).TotalDays >= 30)
            .OrderBy(entity => entity.CreateDate)
            .AsSplitQuery()
            .ToList();

        foreach (var message in guestMessageList)
        {
            _context.Remove(message);
        }

        await _context.SaveChangesAsync();
    }
}
