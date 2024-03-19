using TourishApi.Repository.Interface;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.Transport;
using WebApplication1.Model;
using WebApplication1.Model.Transport;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.InheritanceRepo.Transport
{
    public class AirPlaneRepository : IBaseRepository<AirPlaneModel>
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 5;
        public AirPlaneRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Add(AirPlaneModel addModel)
        {

            var addValue = new PlaneAirline
            {
                BranchName = addModel.BranchName,
                HotlineNumber = addModel.HotlineNumber,
                SupportEmail = addModel.SupportEmail,
                HeadQuarterAddress = addModel.HeadquarterAddress,
                Description = addModel.Description,
                DiscountAmount = addModel.DiscountAmount,
                DiscountFloat = addModel.DiscountFloat,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };
            _context.Add(addValue);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I121",
                // Create type success               
            };

        }

        public Response Delete(Guid id)
        {
            var deleteEntity = _context.PlaneAirlineList.FirstOrDefault((entity
               => entity.Id == id));
            if (deleteEntity != null)
            {
                _context.Remove(deleteEntity);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I123",
                // Delete type success               
            };
        }

        public Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            var entityQuery = _context.PlaneAirlineList.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                entityQuery = entityQuery.Where(entity => entity.BranchName.Contains(search));
            }
            #endregion

            #region Sorting
            entityQuery = entityQuery.OrderBy(entity => entity.BranchName);

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
            var result = PaginatorModel<PlaneAirline>.Create(entityQuery, page, pageSize);
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
            var entity = _context.PlaneAirlineList.FirstOrDefault((entity
                => entity.Id == id));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response getByName(String name)
        {
            var entity = _context.PlaneAirlineList.FirstOrDefault((entity
                => entity.BranchName == name));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response Update(AirPlaneModel entityModel)
        {
            var entity = _context.PlaneAirlineList.FirstOrDefault((entity
                => entity.Id == entityModel.Id));
            if (entity != null)
            {
                entity.UpdateDate = DateTime.UtcNow;
                entity.BranchName = entityModel.BranchName;
                entity.HotlineNumber = entityModel.HotlineNumber;
                entity.SupportEmail = entityModel.SupportEmail;
                entity.HeadQuarterAddress = entityModel.HeadquarterAddress;
                entity.Description = entityModel.Description;
                entity.DiscountAmount = entityModel.DiscountAmount;
                entity.DiscountFloat = entityModel.DiscountFloat;
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I122",
                // Update type success               
            };
        }
    }
}
