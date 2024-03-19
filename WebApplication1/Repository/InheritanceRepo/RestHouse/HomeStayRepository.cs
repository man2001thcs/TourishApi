using TourishApi.Repository.Interface;

using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.RestHouse;
using WebApplication1.Model;
using WebApplication1.Model.RestHouse;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.InheritanceRepo.RestHoouse
{
    public class HomeStayRepository : IBaseRepository<HomeStayModel>
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 5;
        public HomeStayRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Add(HomeStayModel addModel)
        {

            var addValue = new HomeStay
            {
                PlaceBranch = addModel.PlaceBranch,
                HotlineNumber = addModel.HotlineNumber,
                SupportEmail = addModel.SupportEmail,
                HeadQuarterAddress = addModel.HeadQuarterAddress,
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
                MessageCode = "I221",
                // Create type success               
            };

        }

        public Response Delete(Guid id)
        {
            var deleteEntity = _context.HomeStayList.FirstOrDefault((entity
               => entity.Id == id));
            if (deleteEntity != null)
            {
                _context.Remove(deleteEntity);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I223",
                // Delete type success               
            };
        }

        public Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            var entityQuery = _context.HomeStayList.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                entityQuery = entityQuery.Where(entity => entity.PlaceBranch.Contains(search));
            }
            #endregion

            #region Sorting
            entityQuery = entityQuery.OrderBy(entity => entity.PlaceBranch);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.PlaceBranch);
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
            var result = PaginatorModel<HomeStay>.Create(entityQuery, page, pageSize);
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
            var entity = _context.HomeStayList.FirstOrDefault((entity
                => entity.Id == id));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response getByName(String name)
        {
            var entity = _context.HomeStayList.FirstOrDefault((entity
                => entity.PlaceBranch == name));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response Update(HomeStayModel entityModel)
        {
            var entity = _context.HomeStayList.FirstOrDefault((entity
                => entity.Id == entityModel.Id));
            if (entity != null)
            {
                entity.UpdateDate = DateTime.UtcNow;
                entity.PlaceBranch = entityModel.PlaceBranch;
                entity.HotlineNumber = entityModel.HotlineNumber;
                entity.SupportEmail = entityModel.SupportEmail;
                entity.HeadQuarterAddress = entityModel.HeadQuarterAddress;
                entity.Description = entityModel.Description;
                entity.DiscountAmount = entityModel.DiscountAmount;
                entity.DiscountFloat = entityModel.DiscountFloat;
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I222",
                // Update type success               
            };
        }
    }
}
