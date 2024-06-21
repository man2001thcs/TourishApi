namespace TourishApi.Task;

using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.Schedule;

public class RemoveOldMessageConTask
{
    private readonly MyDbContext _context;

    public RemoveOldMessageConTask(MyDbContext _context)
    {
        this._context = _context;
    }

    public async Task RemoveOldConn()
    {
        var guestHisConList = _context
            .GuestMessageConHisList.Include(entity => entity.AdminCon)
            .Include(entity => entity.GuestCon)
            .Where(entity => (DateTime.UtcNow - entity.CreateDate).TotalDays >= 30)
            .OrderBy(entity => entity.CreateDate)
            .AsSplitQuery()
            .ToList();

        foreach (var guestCon in guestHisConList)
        {
            _context.Remove(guestCon.AdminCon);
            _context.Remove(guestCon.GuestCon);
            _context.Remove(guestCon);
        }

        await _context.SaveChangesAsync();
    }
}
