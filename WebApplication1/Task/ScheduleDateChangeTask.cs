﻿namespace TourishApi.Task;

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
            Where(entity => entity.PlanStatus == WebApplication1.Data.PlanStatus.OnGoing).
            Include(entity => entity.EatSchedules).
            Include(entity => entity.StayingSchedules).
            Include(entity => entity.MovingSchedules).ToList();

        foreach (var item in tourishPlanList)
        {
            var today = DateTime.UtcNow;

            foreach (var plan in item.EatSchedules)
            {
                if (plan.EndDate != null && (int)plan.Status < (int)WebApplication1.Data.Schedule.EatScheduleStatus.Completed)
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
                if (plan.EndDate != null && (int)plan.Status < (int)WebApplication1.Data.Schedule.MovingScheduleStatus.Completed)
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
                if (plan.EndDate != null && (int)plan.Status < (int)WebApplication1.Data.Schedule.StayingScheduleStatus.Completed)
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
            var isAllScheduleCanceled = true;

            foreach (var plan in item.EatSchedules)
            {
                if ((int)plan.Status < (int)WebApplication1.Data.Schedule.EatScheduleStatus.Completed)
                {

                    isScheduleCompleted = false;
                }

                if ((int)plan.Status != (int)WebApplication1.Data.Schedule.EatScheduleStatus.Cancelled)
                {

                    isAllScheduleCanceled = false;
                }
            }

            foreach (var plan in item.MovingSchedules)
            {
                if ((int)plan.Status < (int)WebApplication1.Data.Schedule.MovingScheduleStatus.Completed)
                {
                    isScheduleCompleted = false;
                }

                if ((int)plan.Status != (int)WebApplication1.Data.Schedule.MovingScheduleStatus.Cancelled)
                {

                    isAllScheduleCanceled = false;
                }
            }

            foreach (var plan in item.StayingSchedules)
            {
                if ((int)plan.Status < (int)WebApplication1.Data.Schedule.StayingScheduleStatus.Completed)
                {
                    isScheduleCompleted = false;
                }

                if ((int)plan.Status != (int)WebApplication1.Data.Schedule.StayingScheduleStatus.Cancelled)
                {

                    isAllScheduleCanceled = false;
                }
            }

            if (isScheduleCompleted)
            {
                item.PlanStatus = WebApplication1.Data.PlanStatus.Complete;
            }

            if (isAllScheduleCanceled && (item.StayingSchedules.Count() > 0
                || item.MovingSchedules.Count() > 0
                || item.EatSchedules.Count() > 0))
            {
                item.PlanStatus = WebApplication1.Data.PlanStatus.Cancel;
            }
            await _context.SaveChangesAsync();

        }
    }
}
