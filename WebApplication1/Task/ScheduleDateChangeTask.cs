namespace TourishApi.Task;

using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.DbContextFile;
using System.Threading.Tasks;

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

        foreach (var item in tourishPlanList)
        {
            var today = DateTime.UtcNow;

            foreach (var plan in item.EatSchedules)
            {
                if (plan.EndDate != null && plan.Status != WebApplication1.Data.Schedule.EatScheduleStatus.Completed)
                {
                    var endDate = plan.EndDate;
                    if (DateTime.Compare((DateTime)endDate, today) < 0)
                    {
                        plan.Status = WebApplication1.Data.Schedule.EatScheduleStatus.Completed;
                    }
                }
            }

            foreach (var plan in item.MovingSchedules)
            {
                if (plan.EndDate != null && plan.Status != WebApplication1.Data.Schedule.MovingScheduleStatus.Completed)
                {
                    var endDate = plan.EndDate;
                    if (DateTime.Compare((DateTime)endDate, today) < 0)
                    {
                        plan.Status = WebApplication1.Data.Schedule.MovingScheduleStatus.Completed;
                    }
                }

            }

            foreach (var plan in item.StayingSchedules)
            {
                if (plan.EndDate != null && plan.Status != WebApplication1.Data.Schedule.StayingScheduleStatus.Completed)
                {
                    var endDate = plan.EndDate;
                    if (DateTime.Compare((DateTime)endDate, today) < 0)
                    {
                        plan.Status = WebApplication1.Data.Schedule.StayingScheduleStatus.Completed;
                    }
                }
            }

            await _context.SaveChangesAsync();

            var isScheduleCompleted = true;

            foreach (var plan in item.EatSchedules)
            {
                if (plan.Status != WebApplication1.Data.Schedule.EatScheduleStatus.Completed)
                {
                    isScheduleCompleted = false;
                }
            }

            foreach (var plan in item.MovingSchedules)
            {
                if (plan.Status != WebApplication1.Data.Schedule.MovingScheduleStatus.Completed)
                {
                    isScheduleCompleted = false;
                }
            }

            foreach (var plan in item.StayingSchedules)
            {
                if (plan.Status != WebApplication1.Data.Schedule.StayingScheduleStatus.Completed)
                {
                    isScheduleCompleted = false;
                }
            }

            if (isScheduleCompleted)
            {
                item.PlanStatus = WebApplication1.Data.PlanStatus.Complete;
            }
            await _context.SaveChangesAsync();

        }
    }
}
