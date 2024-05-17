namespace TourishApi.Task;

using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication1.Data.DbContextFile;

public class ScheduleDateChangeTask
{
    private readonly MyDbContext _context;
    public ScheduleDateChangeTask(MyDbContext _context)
    {
        this._context = _context;
    }
    public async Task ScheduleDateDueTask()
    {
        var tourishPlanList = _context.TourishPlan.            
            Include(entity => entity.EatSchedules).
            Include(entity => entity.StayingSchedules).
            Include(entity => entity.MovingSchedules).ToList();


    }
}
