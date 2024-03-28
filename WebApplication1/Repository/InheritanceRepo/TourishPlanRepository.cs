using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.Schedule;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;
using WebApplication1.Service;
namespace WebApplication1.Repository.InheritanceRepo;
public class TourishPlanRepository : ITourishPlanRepository
{
    private readonly MyDbContext _context;
    private readonly IBlobService blobService;
    public static int PAGE_SIZE { get; set; } = 5;
    private readonly char[] delimiter = new char[] { ';' };
    public TourishPlanRepository(MyDbContext _context, IBlobService blobService)
    {
        this._context = _context;
        this.blobService = blobService;
    }

    public async Task<Response> Add(TourishPlanInsertModel entityModel, String id)
    {

        var tourishPlan = new TourishPlan
        {
            TourName = entityModel.TourName,
            StartingPoint = entityModel.StartingPoint,
            EndPoint = entityModel.EndPoint,
            RemainTicket = entityModel.RemainTicket,
            TotalTicket = entityModel.TotalTicket,
            SupportNumber = entityModel.SupportNumber,
            PlanStatus = entityModel.PlanStatus,
            StartDate = entityModel.StartDate,
            EndDate = entityModel.EndDate,

            // Description = entityModel.Description,

            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
        };

        var tourishInterest = new TourishInterest();
        var tourishInterestList = new List<TourishInterest>();

        if (id != null)
        {
            var user = _context.Users
                .SingleOrDefault(u => u.Id.ToString() == id);

            tourishInterest = new TourishInterest
            {
                InterestStatus = InterestStatus.Creator,
                User = user,
                TourishPlan = tourishPlan,
                UpdateDate = DateTime.UtcNow
            };

            tourishInterestList.Add(tourishInterest);

            tourishPlan.TourishInterestList = tourishInterestList;
        }

        tourishPlan.TourishInterestList = tourishInterestList;

        if (entityModel.TourishCategoryRelations != null)
        {
            tourishPlan.TourishCategoryRelations = entityModel.TourishCategoryRelations;
        }

        await _context.AddAsync(tourishPlan);
        await _context.SaveChangesAsync();
        await blobService.UploadStringBlobAsync("tourish-content-container", entityModel.Description ?? "", "text/plain", tourishPlan.Id.ToString() ?? "" + ".txt");

        if (!String.IsNullOrEmpty(entityModel.EatingScheduleString))
        {
            await AddEatSchedule(tourishPlan.Id, entityModel.EatingScheduleString);
        }

        if (!String.IsNullOrEmpty(entityModel.MovingScheduleString))
        {
            await AddMovingSchedule(tourishPlan.Id, entityModel.MovingScheduleString);
        }

        if (!String.IsNullOrEmpty(entityModel.StayingScheduleString))
        {
            await AddStayingSchedule(tourishPlan.Id, entityModel.StayingScheduleString);
        }     

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

    public Response GetAll(string? search, string? category, string? sortBy, int page = 1, int pageSize = 5)
    {
        var entityQuery = _context.TourishPlan.Include(entity => entity.MovingSchedules).
            Include(entity => entity.EatSchedules).
            Include(entity => entity.StayingSchedules).
            Include(entity => entity.TourishCategoryRelations).
            ThenInclude(entity => entity.TourishCategory).
             Include(entity => entity.TotalReceipt).
             ThenInclude(entity => entity.FullReceiptList).
            AsQueryable();

        #region Filtering
        if (!string.IsNullOrEmpty(search))
        {
            entityQuery = entityQuery.Where(entity => entity.TourName.Contains(search));
        }

        if (!string.IsNullOrEmpty(category))
        {
            entityQuery = entityQuery.Where(entity => entity.TourishCategoryRelations
            .Count(categoryRelation => (categoryRelation.TourishCategory != null) ? categoryRelation.TourishCategory.Name.Contains(category) : false) >= 1);
        }

        #endregion

        #region Sorting
        //Default sort by Name (TenHh)
        entityQuery = entityQuery.OrderByDescending(entity => entity.TourName);

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
            Include(entity => entity.MovingSchedules).
            Include(entity => entity.TourishCategoryRelations).
            ThenInclude(entity => entity.TourishCategory).
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

    public async Task<Response> Update(TourishPlanUpdateModel entityModel, String id)
    {
        var entity = _context.TourishPlan.Include(entity => entity.TourishInterestList).FirstOrDefault((entity
            => entity.Id == entityModel.Id));
        if (entity != null)
        {

            entity.TourName = entityModel.TourName ?? entity.TourName;

            entity.TourName = entityModel.TourName;
            // entity.Description = entityModel.Description;
            entity.StartingPoint = entityModel.StartingPoint;
            entity.EndPoint = entityModel.EndPoint;

            entity.RemainTicket = entityModel.RemainTicket;
            entity.TotalTicket = entityModel.TotalTicket;
            entity.SupportNumber = entityModel.SupportNumber;

            entity.PlanStatus = entityModel.PlanStatus;
            entity.StartDate = entityModel.StartDate;
            entity.EndDate = entityModel.EndDate;

            var tourishInterest = new TourishInterest();

            if (id != null)
            {
                var user = _context.Users
                .SingleOrDefault(u => u.Id.ToString() == id);

                if (user != null)
                {
                    tourishInterest = new TourishInterest
                    {
                        InterestStatus = InterestStatus.Modifier,
                        User = user,
                        TourishPlan = entity,
                        UpdateDate = DateTime.UtcNow
                    };

                    if (entity.TourishInterestList == null)
                    {
                        entity.TourishInterestList = new List<TourishInterest>();
                    }

                    if (entity.TourishInterestList.Count(interest => interest.UserId.ToString() == id) <= 0)
                    {
                        entity.TourishInterestList.Add(tourishInterest);
                    }


                }
            }

            if (entityModel.TourishCategoryRelations != null)
            {
                await _context.TourishCategoryRelations.Where(a => a.TourishPlanId == entityModel.Id).ExecuteDeleteAsync();
                await _context.SaveChangesAsync();
                entity.TourishCategoryRelations = entityModel.TourishCategoryRelations;
            }

            entity.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await blobService.UploadStringBlobAsync("tourish-content-container", entityModel.Description ?? "", "text/plain", entityModel.Id.ToString() ?? "" + ".txt");

            if (entityModel.StayingScheduleString != "" && entityModel.StayingScheduleString != null)
            {
                await _context.StayingSchedules.Where(a => a.TourishPlanId == entityModel.Id).ExecuteDeleteAsync();
                await AddStayingSchedule(entity.Id, entityModel.StayingScheduleString);
            }

            if (entityModel.MovingScheduleString != "" && entityModel.MovingScheduleString != null)
            {
                await _context.MovingSchedules.Where(a => a.TourishPlanId == entityModel.Id).ExecuteDeleteAsync();
                await AddMovingSchedule(entity.Id, entityModel.MovingScheduleString);
            }

            if (entityModel.EatingScheduleString != "" && entityModel.EatingScheduleString != null)
            {
                await _context.EatSchedules.Where(a => a.TourishPlanId == entityModel.Id).ExecuteDeleteAsync();
                await AddEatSchedule(entity.Id, entityModel.EatingScheduleString);
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I412",
                // Update type success               
            };
        }
        else
        {
            return new Response
            {
                resultCd = 0,
                MessageCode = "C412",
            };
        }
    }

    public async Task<string> getDescription(string containerName, string blobName)
    {
        return await blobService.GetBlobContentAsync(containerName, blobName);
    }


    public async Task<List<TourishInterest>> getTourInterest(Guid id)
    {
        var entity = await _context.TourishPlan.Where(entity => entity.Id == id).Include(tour => tour.TourishInterestList)
           .FirstOrDefaultAsync();
        if (entity == null) { return null; }

        return entity.TourishInterestList.ToList();
    }

    private async Task AddEatSchedule(Guid tourishPlanId, string FullScheduleString)
    {
        if (!String.IsNullOrEmpty(FullScheduleString))
        {
            dynamic scheduleArray = Newtonsoft.Json.JsonConvert.DeserializeObject(FullScheduleString);

            if (scheduleArray != null)
            {

                foreach (var schedule in scheduleArray)
                {
                    var eatSchedule = new EatSchedule
                    {
                        PlaceName = schedule.placeName,
                        SinglePrice = schedule.singlePrice,
                        Address = schedule.address,
                        SupportNumber = schedule.supportNumber,
                        // Description = schedule.description,
                        RestaurantId = schedule.restaurantId,
                        TourishPlanId = tourishPlanId,
                        Status = schedule.status ?? 0,
                        StartDate = schedule.startDate,
                        EndDate = schedule.endDate,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                    };
                    await _context.AddAsync(eatSchedule);
                    await _context.SaveChangesAsync();

                    await blobService.UploadStringBlobAsync("eatschedule-content-container", (String) schedule.description ?? "Không có thông tin", "text/plain", eatSchedule.Id.ToString() ?? "" + ".txt");
                }
            }
        }

    }

    private async Task AddMovingSchedule(Guid tourishPlanId, string FullScheduleString)
    {

        if (!String.IsNullOrEmpty(FullScheduleString))
        {
            dynamic scheduleArray = Newtonsoft.Json.JsonConvert.DeserializeObject(FullScheduleString);

            if (scheduleArray != null)
            {

                foreach (var schedule in scheduleArray)
                {
                    var movingSchedule = new MovingSchedule
                    {
                        DriverName = schedule.driverName ?? "",
                        VehiclePlate = schedule.vehiclePlate,
                        BranchName = schedule.branchName,
                        VehicleType = schedule.vehicleType,
                        TransportId = schedule.transportId,
                        SinglePrice = schedule.singlePrice,
                        Status = schedule.status ?? 0,
                        PhoneNumber = schedule.phoneNumber,
                        StartingPlace = schedule.startingPlace,
                        HeadingPlace = schedule.headingPlace,
                        TourishPlanId = tourishPlanId,
                        // Description = schedule.description,
                        StartDate = schedule.startDate,
                        EndDate = schedule.endDate,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                    };

                    await _context.AddAsync(movingSchedule);
                    await _context.SaveChangesAsync();
                    await blobService.UploadStringBlobAsync("movingschedule-content-container", (String) schedule.description ?? "Không có thông tin", "text/plain", movingSchedule.Id.ToString() ?? "" + ".txt");
                }

            }
        }
    }

    private async Task AddStayingSchedule(Guid tourishPlanId, string FullScheduleString)
    {

        if (!String.IsNullOrEmpty(FullScheduleString))
        {
            dynamic scheduleArray = Newtonsoft.Json.JsonConvert.DeserializeObject(FullScheduleString);

            if (scheduleArray != null)
            {

                foreach (var schedule in scheduleArray)
                {
                    var stayingSchedule = new StayingSchedule
                    {
                        PlaceName = schedule.placeName,
                        Address = schedule.address,
                        SupportNumber = schedule.supportNumber,
                        SinglePrice = schedule.singlePrice,
                        Status = schedule.status ?? 0,
                        RestHouseBranchId = schedule.restHouseBranchId,
                        RestHouseType = schedule.restHouseType,
                        // Description = schedule.description,
                        TourishPlanId = tourishPlanId,
                        StartDate = schedule.startDate,
                        EndDate = schedule.endDate,

                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                    };

                    await _context.AddAsync(stayingSchedule);
                    await _context.SaveChangesAsync();

                    await blobService.UploadStringBlobAsync("movingschedule-content-container", (String) schedule.description ?? "Không có thông tin", "text/plain", stayingSchedule.Id.ToString() ?? "" + ".txt");
                }
            }
        }
    } 
}
