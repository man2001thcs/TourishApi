using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.Schedule;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;
namespace WebApplication1.Repository.InheritanceRepo;
public class TourishPlanRepository : ITourishPlanRepository
{
    private readonly MyDbContext _context;
    public static int PAGE_SIZE { get; set; } = 5;
    private readonly char[] delimiter = new char[] { ';' };
    public TourishPlanRepository(MyDbContext _context)
    {
        this._context = _context;
    }

    public async Task<Response> Add(TourishPlanInsertModel entityModel)
    {

        var tourishPlan = new TourishPlan
        {
            TourName = entityModel.TourName,
            Description = entityModel.Description,
            StartingPoint = entityModel.StartingPoint,
            EndPoint = entityModel.EndPoint,

            RemainTicket = entityModel.RemainTicket,
            TotalTicket = entityModel.TotalTicket,
            SupportNumber = entityModel.SupportNumber,

            PlanStatus = entityModel.PlanStatus,
            StartDate = entityModel.StartDate,
            EndDate = entityModel.EndDate,

            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
        };

        if (!String.IsNullOrEmpty(entityModel.EatingScheduleString))
        {
            tourishPlan.EatSchedules = this.AddEatSchedule(entityModel.EatingScheduleString);
        }

        if (!String.IsNullOrEmpty(entityModel.MovingScheduleString))
        {
            tourishPlan.MovingSchedules = this.AddMovingSchedule(entityModel.MovingScheduleString);
        }

        if (!String.IsNullOrEmpty(entityModel.StayingScheduleString))
        {
            tourishPlan.StayingSchedules = this.AddStayingSchedule(entityModel.StayingScheduleString);
        }

        _context.Add(tourishPlan);
        await _context.SaveChangesAsync();

        return new Response
        {
            resultCd = 0,
            MessageCode = "I411",
            returnId = tourishPlan.Id,
            // Create type success               
        };

    }

    public Response Delete(Guid id)
    {
        var entity = _context.TourishPlan.FirstOrDefault((entity
          => entity.Id == id));
        if (entity != null)
        {
            _context.Remove(entity);
            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I413",
            // Delete type success               
        };
    }

    public Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
    {
        var entityQuery = _context.TourishPlan.Include(entity => entity.MovingSchedules).
            Include(entity => entity.EatSchedules).
            Include(entity => entity.StayingSchedules).
             Include(entity => entity.TotalReceipt).
             ThenInclude(entity => entity.FullReceiptList).
            AsQueryable();

        #region Filtering
        if (!string.IsNullOrEmpty(search))
        {
            entityQuery = entityQuery.Where(entity => entity.TourName.Contains(search));
        }

        #endregion

        #region Sorting
        //Default sort by Name (TenHh)
        entityQuery = entityQuery.OrderBy(entity => entity.TourName);

        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy)
            {
                case "title_desc":
                    entityQuery = entityQuery.OrderByDescending(entity => entity.TourName);
                    break;
            }
        }
        #endregion

        #region Paging
        var result = PaginatorModel<TourishPlan>.Create(entityQuery, page, PAGE_SIZE);
        #endregion

        var entityVM = new Response
        {
            resultCd = 0,
            Data = result.ToList(),
            count = result.TotalCount,
        };

        return entityVM;
    }

    public Response getById(Guid id)
    {
        var entity = _context.TourishPlan.Where(entity => entity.Id == id).Include(entity => entity.EatSchedules).
            Include(entity => entity.StayingSchedules).
             Include(entity => entity.TotalReceipt).
             ThenInclude(entity => entity.FullReceiptList)
           .FirstOrDefault();
        if (entity == null) { return null; }

        return new Response
        {
            resultCd = 0,
            Data = entity
        };
    }

    public Response getByName(String TourName)
    {
        var entity = _context.TourishPlan.FirstOrDefault((entity
            => entity.TourName == TourName));

        return new Response
        {
            resultCd = 0,
            Data = entity
        };
    }

    public async Task<Response> Update(TourishPlanUpdateModel entityModel)
    {
        var entity = _context.TourishPlan.FirstOrDefault((entity
            => entity.Id == entityModel.Id));
        if (entity != null)
        {

            entity.TourName = entityModel.TourName ?? entity.TourName;

            entity.TourName = entityModel.TourName;
            entity.Description = entityModel.Description;
            entity.StartingPoint = entityModel.StartingPoint;
            entity.EndPoint = entityModel.EndPoint;

            entity.RemainTicket = entityModel.RemainTicket;
            entity.TotalTicket = entityModel.TotalTicket;
            entity.SupportNumber = entityModel.SupportNumber;

            entity.PlanStatus = entityModel.PlanStatus;
            entity.StartDate = entityModel.StartDate;
            entity.EndDate = entityModel.EndDate;

            if (entityModel.StayingScheduleString != "" && entityModel.StayingScheduleString != null)
            {
                await _context.StayingSchedules.Where(a => a.TourishPlanId == entityModel.Id).ExecuteDeleteAsync();
                entity.StayingSchedules = AddStayingSchedule(entityModel.StayingScheduleString);
            }

            if (entityModel.MovingScheduleString != "" && entityModel.MovingScheduleString != null)
            {
                await _context.MovingSchedules.Where(a => a.TourishPlanId == entityModel.Id).ExecuteDeleteAsync();
                entity.MovingSchedules = AddMovingSchedule(entityModel.MovingScheduleString);
            }

            if (entityModel.EatingScheduleString != "" && entityModel.EatingScheduleString != null)
            {
                await _context.EatSchedules.Where(a => a.TourishPlanId == entityModel.Id).ExecuteDeleteAsync();
                entity.EatSchedules = AddEatSchedule(entityModel.EatingScheduleString);
            }

            entity.UpdateDate = DateTime.UtcNow;

            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I412",
            // Update type success               
        };
    }

    private List<EatSchedule> AddEatSchedule(string FullScheduleString)
    {
        var scheduleList = new List<EatSchedule>();

        if (!String.IsNullOrEmpty(FullScheduleString))
        {
            dynamic scheduleArray = Newtonsoft.Json.JsonConvert.DeserializeObject(FullScheduleString);

            if (scheduleArray != null)
            {
                if (scheduleArray.length > 0)
                {
                    foreach (var schedule in scheduleArray)
                    {
                        scheduleList.Add(new EatSchedule
                        {
                            PlaceName = schedule.placeName,
                            Address = schedule.address,
                            SupportNumber = schedule.supportNumber,
                            Description = schedule.description,
                            StartDate = schedule.startDate,
                            EndDate = schedule.endDate,
                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow,
                        });
                    }
                }
            }
        }

        return scheduleList;
    }

    private List<MovingSchedule> AddMovingSchedule(string FullScheduleString)
    {
        var scheduleList = new List<MovingSchedule>();

        if (!String.IsNullOrEmpty(FullScheduleString))
        {
            dynamic scheduleArray = Newtonsoft.Json.JsonConvert.DeserializeObject(FullScheduleString);

            if (scheduleArray != null)
            {
                if (scheduleArray.length > 0)
                {
                    foreach (var schedule in scheduleArray)
                    {
                        scheduleList.Add(new MovingSchedule
                        {
                            DriverName = schedule.driverName ?? "",
                            VehiclePlate = schedule.vehiclePlate,

                            VehicleType = schedule.vehicleType,
                            TransportId = schedule.transportId,

                            PhoneNumber = schedule.phoneNumber,
                            StartingPlace = schedule.startingPlace,
                            HeadingPlace = schedule.headingPlace,

                            Description = schedule.description,
                            StartDate = schedule.startDate,
                            EndDate = schedule.endDate,
                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow,
                        });
                    }
                }
            }
        }

        return scheduleList;
    }

    private List<StayingSchedule> AddStayingSchedule(string FullScheduleString)
    {
        var scheduleList = new List<StayingSchedule>();

        if (!String.IsNullOrEmpty(FullScheduleString))
        {
            dynamic scheduleArray = Newtonsoft.Json.JsonConvert.DeserializeObject(FullScheduleString);

            if (scheduleArray != null)
            {
                if (scheduleArray.length > 0)
                {
                    foreach (var schedule in scheduleArray)
                    {
                        scheduleList.Add(new StayingSchedule
                        {
                            PlaceName = schedule.placeName,
                            Address = schedule.address,
                            SupportNumber = schedule.supportNumber,

                            RestHouseBranchId = schedule.restHouseBranchId,
                            RestHouseType = schedule.restHouseType,
                            Description = schedule.description,

                            StartDate = schedule.startDate,
                            EndDate = schedule.endDate,

                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow,
                        });
                    }
                }
            }
        }

        return scheduleList;
    }
}
