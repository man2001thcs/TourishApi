using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.InheritanceRepo
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 5;
        public UserRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Delete(Guid id)
        {
            var deleteEntity = _context.Users.FirstOrDefault((entity
               => entity.Id == id));
            if (deleteEntity != null)
            {
                _context.Remove(deleteEntity);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I003",
                // Delete type success               
            };
        }

        public Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            var entityQuery = _context.Users.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                entityQuery = entityQuery.Where(entity => entity.FullName.Contains(search));
            }
            #endregion

            #region Sorting
            entityQuery = entityQuery.OrderBy(entity => entity.FullName);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.FullName);
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
            var result = PaginatorModel<User>.Create(entityQuery, page, pageSize);
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
            var entity = _context.Users.FirstOrDefault((entity
                => entity.Id == id));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response getByName(String name)
        {
            var entity = _context.Users.FirstOrDefault((entity
                => entity.UserName == name));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public async Task<Response> UpdateInfo(UserUpdateModel model)
        {
            var userExist = _context.Users.FirstOrDefault(p => p.UserName == model.UserName);
            if (userExist != null) //không đúng
            {
                userExist.UpdateDate = DateTime.UtcNow;
                userExist.FullName = model.FullName ?? userExist.FullName;
                userExist.PhoneNumber = model.PhoneNumber ?? userExist.PhoneNumber;
                userExist.Address = model.Address ?? userExist.Address;
                userExist.Role = model.Role ?? userExist.Role;

                if (userExist.Password != "None")
                {
                    userExist.Email = model.Email;
                }

                await _context.SaveChangesAsync();

                return new Response
                {
                    resultCd = 0,
                    MessageCode = "I011",
                };
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "C011",
            };
        }

        public async Task<Response> UpdatePassword(UserUpdatePasswordModel model)
        {
            var userExist = _context.Users.FirstOrDefault(p => p.UserName == model.UserName);
            if (userExist != null) //không đúng
            {
                userExist.UpdateDate = DateTime.UtcNow;

                if (userExist.Password != "None")
                {
                    userExist.Password = model.NewPassword;
                }

                await _context.SaveChangesAsync();

                return new Response
                {
                    resultCd = 0,
                    MessageCode = "I012",
                };
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "C012",
            };
        }
    }
}
