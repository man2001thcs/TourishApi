using Microsoft.EntityFrameworkCore;
using TourishApi.Repository.Interface;
using WebApplication1.Data.Connection;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.Connection;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.InheritanceRepo.Connect
{
    public class GuestMessageConHistoryRepository : IBaseRepository<GuestMessageConHistoryModel>
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 5;
        public GuestMessageConHistoryRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Add(GuestMessageConHistoryModel addModel)
        {

            var addValue = new GuestMessageConHistory
            {
                GuestConId = addModel.Id,
                AdminConId = addModel.AdminConId,
                CreateDate = DateTime.UtcNow
            };
            _context.Add(addValue);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I1111",
                // Create type success               
            };

        }

        public Response Delete(Guid id)
        {
            var deleteEntity = _context.GuestMessageConHisList.FirstOrDefault((entity
               => entity.Id == id));
            if (deleteEntity != null)
            {
                _context.Remove(deleteEntity);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I1113",
                // Delete type success               
            };
        }

        public Response GetAll(string? search, int? type, string? sortBy, int page = 1, int pageSize = 5)
        {
            var entityQuery = _context.GuestMessageConHisList.Include(entity => entity.GuestCon)
                .Include(entity => entity.AdminCon).ThenInclude(entity => entity.Admin).AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                entityQuery = entityQuery.Where(entity => entity.AdminConId.ToString().Contains(search));
            }
            #endregion

            #region Sorting
            entityQuery = entityQuery.OrderByDescending(entity => entity.CreateDate);
            #endregion

            #region Paging
            var result = PaginatorModel<GuestMessageConHistory>.Create(entityQuery, page, pageSize);
            #endregion

            var resultDto = result.Select(guestConHis => new GuestMessageConHistoryDTOModel
            {
                Id = guestConHis.Id,
                GuestMessageCon = new GuestMessageConDTOModel
                {
                    Id = guestConHis.GuestCon.Id,
                    Connected = guestConHis.GuestCon.Connected,
                    GuestEmail = guestConHis.GuestCon.GuestEmail,
                    GuestName = guestConHis.GuestCon.GuestName,
                    GuestPhoneNumber = guestConHis.GuestCon.GuestPhoneNumber,
                    ConnectionID = guestConHis.GuestCon.ConnectionID,
                    UserAgent = guestConHis.GuestCon.UserAgent,
                    CreateDate = guestConHis.GuestCon.CreateDate
                },
                AdminMessageCon = guestConHis.AdminCon != null ? new AdminMessageConDTOModel
                {
                    Id = guestConHis.AdminCon.Id,
                    Connected = guestConHis.AdminCon.Connected,
                    ConnectionID = guestConHis.AdminCon.ConnectionID,
                    AdminFullName = guestConHis.AdminCon.Admin.FullName,
                    AdminId = guestConHis.AdminCon.AdminId.Value,
                    UserAgent = guestConHis.AdminCon.UserAgent,
                    CreateDate = guestConHis.AdminCon.CreateDate
                } : null,
                Status = guestConHis.GuestCon.Connected ? (guestConHis.AdminCon != null ? 1 : 2) : 0,
                CreateDate = guestConHis.CreateDate,
                CloseDate = guestConHis.CloseDate,
            }).OrderByDescending(entity => entity.Status).ToList();

            var entityVM = new Response
            {
                resultCd = 0,
                Data = resultDto,
                count = result.TotalCount,
            };
            return entityVM;

        }

        public Response GetAllForAdmin(string? search, int? type, string? sortBy, int page = 1, int pageSize = 5)
        {
            var entityQuery = _context.GuestMessageConHisList.Include(entity => entity.GuestCon)
                .Include(entity => entity.AdminCon).ThenInclude(entity => entity.Admin).AsQueryable();

            #region Filtering
            entityQuery = entityQuery.Where(entity => entity.AdminCon != null);

            if (!string.IsNullOrEmpty(search))
            {
                entityQuery = entityQuery.Where(entity => entity.AdminConId.ToString().Contains(search));
            }
            #endregion

            #region Sorting
            entityQuery = entityQuery.OrderByDescending(entity => entity.CreateDate);
            #endregion

            #region Paging
            var result = PaginatorModel<GuestMessageConHistory>.Create(entityQuery, page, pageSize);
            #endregion

            var resultDto = result.Select(guestConHis => new GuestMessageConHistoryDTOModel
            {
                Id = guestConHis.Id,
                GuestMessageCon = new GuestMessageConDTOModel
                {
                    Id = guestConHis.GuestCon.Id,
                    Connected = guestConHis.GuestCon.Connected,
                    GuestEmail = guestConHis.GuestCon.GuestEmail,
                    GuestName = guestConHis.GuestCon.GuestName,
                    GuestPhoneNumber = guestConHis.GuestCon.GuestPhoneNumber,
                    ConnectionID = guestConHis.GuestCon.ConnectionID,
                    UserAgent = guestConHis.GuestCon.UserAgent,
                    CreateDate = guestConHis.GuestCon.CreateDate
                },
                AdminMessageCon = guestConHis.AdminCon != null ? new AdminMessageConDTOModel
                {
                    Id = guestConHis.AdminCon.Id,
                    Connected = guestConHis.AdminCon.Connected,
                    ConnectionID = guestConHis.AdminCon.ConnectionID,
                    AdminFullName = guestConHis.AdminCon.Admin.FullName,
                    AdminId = guestConHis.AdminCon.AdminId.Value,
                    UserAgent = guestConHis.AdminCon.UserAgent,
                    CreateDate = guestConHis.AdminCon.CreateDate
                } : null,
                Status = guestConHis.GuestCon.Connected ? (guestConHis.AdminCon != null ? 1 : 2) : 0,
                CreateDate = guestConHis.CreateDate,
                CloseDate = guestConHis.CloseDate,
            }).OrderByDescending(entity => entity.Status).ToList();

            var entityVM = new Response
            {
                resultCd = 0,
                Data = resultDto,
                count = result.TotalCount,
            };
            return entityVM;

        }

        public Response getById(Guid id)
        {
            var entity = _context.GuestMessageConHisList.FirstOrDefault((entity
                => entity.Id == id));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response getByGuestConId(string connectionId)
        {
            var entity = _context.GuestMessageConHisList.Include(entity => entity.GuestCon)
                .ThenInclude(entity => entity.GuestMessages)
                .Include(entity => entity.AdminCon).ThenInclude(entity => entity.Admin)
                .Include(entity => entity.AdminCon).ThenInclude(entity => entity.GuestMessages).Where(entity
                => entity.GuestCon.ConnectionID == connectionId);

            var resultDto = entity.Select(guestConHis => new GuestMessageConHistoryDTOModel
            {
                Id = guestConHis.Id,
                GuestMessageCon = new GuestMessageConDTOModel
                {
                    Id = guestConHis.GuestCon.Id,
                    Connected = guestConHis.GuestCon.Connected,
                    GuestEmail = guestConHis.GuestCon.GuestEmail,
                    GuestName = guestConHis.GuestCon.GuestName,
                    GuestPhoneNumber = guestConHis.GuestCon.GuestPhoneNumber,
                    GuestMessages = guestConHis.GuestCon.GuestMessages,
                    ConnectionID = guestConHis.GuestCon.ConnectionID,
                    UserAgent = guestConHis.GuestCon.UserAgent,
                    CreateDate = guestConHis.GuestCon.CreateDate
                },
                AdminMessageCon = guestConHis.AdminCon != null ? new AdminMessageConDTOModel
                {
                    Id = guestConHis.AdminCon.Id,
                    Connected = guestConHis.AdminCon.Connected,
                    ConnectionID = guestConHis.AdminCon.ConnectionID,
                    AdminFullName = guestConHis.AdminCon.Admin.FullName,
                    GuestMessages = guestConHis.AdminCon.GuestMessages,
                    AdminId = guestConHis.AdminCon.AdminId.Value,
                    UserAgent = guestConHis.AdminCon.UserAgent,
                    CreateDate = guestConHis.AdminCon.CreateDate
                } : null,
                Status = guestConHis.GuestCon.Connected ? (guestConHis.AdminCon != null ? 1 : 2) : 0,
                CreateDate = guestConHis.CreateDate,
                CloseDate = guestConHis.CloseDate,
            }).FirstOrDefault();

            return new Response
            {
                resultCd = 0,
                Data = resultDto
            };
        }

        public Response getByName(String name)
        {
            var entity = _context.GuestMessageConHisList.FirstOrDefault((entity
                => entity.AdminConId.ToString() == name));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response Update(GuestMessageConHistoryModel entityModel)
        {
            var entity = _context.GuestMessageConHisList.FirstOrDefault((entity
                => entity.Id == entityModel.Id));
            if (entity != null)
            {
                entity.AdminConId = entityModel.AdminConId;
                entity.GuestConId = entityModel.GuestConId;

                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I1112",
                // Update type success               
            };
        }
    }
}
