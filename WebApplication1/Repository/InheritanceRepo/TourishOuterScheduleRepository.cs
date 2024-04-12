using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.Schedule;
using WebApplication1.Model;
using WebApplication1.Model.Schedule;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.InheritanceRepo
{
    public class TourishOuterScheduleRepository
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 5;
        public TourishOuterScheduleRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response AddEatSchedule(EatScheduleModel addModel)
        {

            var addValue = new EatSchedule
            {
                PlaceName = addModel.PlaceName,
                RestaurantId = addModel.RestaurantId,
                SinglePrice = addModel.SinglePrice,
                Address = addModel.Address,
                SupportNumber = addModel.SupportNumber,
                TourishPlanId = null,
                Status = addModel.Status,
                StartDate = addModel.StartDate,
                EndDate = addModel.EndDate,

                Description = addModel.Description,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };
            _context.Add(addValue);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I431",
                // Create type success               
            };
        }

        public Response AddMovingSchedule(MovingScheduleModel addModel)
        {

            var addValue = new MovingSchedule
            {
                BranchName = addModel.BranchName,
                HeadingPlace = addModel.HeadingPlace,
                StartingPlace = addModel.StartingPlace,
                TransportId = addModel.TransportId,
                VehicleType =addModel.VehicleType,
                SinglePrice = addModel.SinglePrice,

                TourishPlanId = null,
                Status = addModel.Status,
                StartDate = addModel.StartDate,
                EndDate = addModel.EndDate,

                Description = addModel.Description,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };
            _context.Add(addValue);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I431",
                // Create type success               
            };
        }

        public Response AddStayingSchedule(StayingScheduleModel addModel)
        {

            var addValue = new StayingSchedule
            {
                PlaceName = addModel.PlaceName,
                RestHouseBranchId = addModel.RestHouseBranchId,
                RestHouseType = addModel.RestHouseType,
                SinglePrice = addModel.SinglePrice,
                Address = addModel.Address,
                SupportNumber = addModel.SupportNumber,
                TourishPlanId = null,
                Status = addModel.Status,
                StartDate = addModel.StartDate,
                EndDate = addModel.EndDate,

                Description = addModel.Description,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };
            _context.Add(addValue);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I431",
                // Create type success               
            };
        }

        public Response DeleteEatSchedule(Guid id)
        {
            var deleteEntity = _context.EatSchedules.FirstOrDefault((entity
               => entity.Id == id));
            if (deleteEntity != null)
            {
                _context.Remove(deleteEntity);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I433",
                // Delete type success               
            };
        }

        public Response DeleteMovingSchedule(Guid id)
        {
            var deleteEntity = _context.MovingSchedules.FirstOrDefault((entity
               => entity.Id == id));
            if (deleteEntity != null)
            {
                _context.Remove(deleteEntity);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I433",
                // Delete type success               
            };
        }

        public Response DeleteStayingSchedule(Guid id)
        {
            var deleteEntity = _context.StayingSchedules.FirstOrDefault((entity
               => entity.Id == id));
            if (deleteEntity != null)
            {
                _context.Remove(deleteEntity);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I433",
                // Delete type success               
            };
        }

        public Response GetAllEatSchedule(string? search, int? type, string? sortBy, int page = 1, int pageSize = 5)
        {
            var entityQuery = _context.EatSchedules.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                entityQuery = entityQuery.Where(entity => entity.PlaceName.Contains(search));
            }
            #endregion

            #region Sorting
            entityQuery = entityQuery.OrderByDescending(entity => entity.UpdateDate);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.PlaceName);
                        break;
                    case "updateDate_asc":
                        entityQuery = entityQuery.OrderBy(entity => entity.UpdateDate);
                        break;
                    case "updateDate_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.UpdateDate);
                        break;
                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<EatSchedule>.Create(entityQuery, page, pageSize);
            #endregion

            var entityVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return entityVM;

        }

        public Response GetAllMovingSchedule(string? search, int? type, string? sortBy, int page = 1, int pageSize = 5)
        {
            var entityQuery = _context.MovingSchedules.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                entityQuery = entityQuery.Where(entity => entity.BranchName
                .Contains(search));
            }
            #endregion

            #region Sorting
            entityQuery = entityQuery.OrderByDescending(entity => entity.UpdateDate);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.BranchName);
                        break;
                    case "updateDate_asc":
                        entityQuery = entityQuery.OrderBy(entity => entity.UpdateDate);
                        break;
                    case "updateDate_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.UpdateDate);
                        break;
                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<MovingSchedule>.Create(entityQuery, page, pageSize);
            #endregion

            var entityVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return entityVM;

        }

        public Response GetAllStayingSchedule(string? search, int? type, string? sortBy, int page = 1, int pageSize = 5)
        {
            var entityQuery = _context.StayingSchedules.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                entityQuery = entityQuery.Where(entity => entity.PlaceName
                .Contains(search));
            }
            #endregion

            #region Sorting
            entityQuery = entityQuery.OrderByDescending(entity => entity.UpdateDate);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.PlaceName);
                        break;
                    case "updateDate_asc":
                        entityQuery = entityQuery.OrderBy(entity => entity.UpdateDate);
                        break;
                    case "updateDate_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.UpdateDate);
                        break;
                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<StayingSchedule>.Create(entityQuery, page, pageSize);
            #endregion

            var entityVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return entityVM;

        }

        public Response getByEatScheduleId(Guid id)
        {
            var entity = _context.EatSchedules.FirstOrDefault((entity
                => entity.Id == id));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response getByNameEatSchedule(String name)
        {
            var entity = _context.EatSchedules.FirstOrDefault((entity
                => entity.PlaceName == name));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response UpdateEatSchedule(EatScheduleModel entityModel)
        {
            var entity = _context.EatSchedules.FirstOrDefault((entity
                => entity.Id == entityModel.Id));
            if (entity != null)
            {
                entity.UpdateDate = DateTime.UtcNow;
                entity.Description = entityModel.Description;
                entity.PlaceName = entityModel.PlaceName;
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I432",
                // Update type success               
            };
        }

        public Response getByStayingScheduleId(Guid id)
        {
            var entity = _context.StayingSchedules.FirstOrDefault((entity
                => entity.Id == id));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response getByNameStayingSchedule(String name)
        {
            var entity = _context.StayingSchedules.FirstOrDefault((entity
                => entity.PlaceName == name));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response UpdateStayingSchedule(StayingScheduleModel entityModel)
        {
            var entity = _context.StayingSchedules.FirstOrDefault((entity
                => entity.Id == entityModel.Id));
            if (entity != null)
            {
                entity.UpdateDate = DateTime.UtcNow;
                entity.Description = entityModel.Description;
                entity.PlaceName = entityModel.PlaceName;
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I432",
                // Update type success               
            };
        }


        public Response getByMovingScheduleId(Guid id)
        {
            var entity = _context.StayingSchedules.FirstOrDefault((entity
                => entity.Id == id));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response getByNameMovingSchedule(String name)
        {
            var entity = _context.StayingSchedules.FirstOrDefault((entity
                => entity.PlaceName == name));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response UpdateMovingSchedule(MovingScheduleModel entityModel)
        {
            var entity = _context.MovingSchedules.FirstOrDefault((entity
                => entity.Id == entityModel.Id));
            if (entity != null)
            {
                entity.UpdateDate = DateTime.UtcNow;
                entity.Description = entityModel.Description;
                entity.BranchName = entityModel.BranchName;
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I432",
                // Update type success               
            };
        }
    }
}
