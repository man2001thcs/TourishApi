using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.InheritanceRepo
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 5;
        public AuthorRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Add(AuthorModel authorModel)
        {

            var author = new Author
            {
                Name = authorModel.Name,
                PhoneNumber = authorModel.PhoneNumber,
                Email = authorModel.Email,
                Address = authorModel.Address,
                Description = authorModel.Description,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };
            _context.Add(author);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I401",
                // Create type success               
            };

        }

        public Response Delete(Guid id)
        {
            var author = _context.Authors.FirstOrDefault((author
               => author.Id == id));
            if (author != null)
            {
                _context.Remove(author);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I403",
                // Delete type success               
            };
        }

        public Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            var authorQuery = _context.Authors.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                authorQuery = authorQuery.Where(author => author.Name.Contains(search));
            }
            #endregion

            #region Sorting
            //Default sort by Name (TenHh)
            authorQuery = authorQuery.OrderBy(author => author.Name);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        authorQuery = authorQuery.OrderByDescending(author => author.Name);
                        break;
                    case "updateDate_asc":
                        authorQuery = authorQuery.OrderBy(author => author.UpdateDate);
                        break;
                    case "updateDate_desc":
                        authorQuery = authorQuery.OrderByDescending(author => author.UpdateDate);
                        break;
                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<Author>.Create(authorQuery, page, pageSize);
            #endregion

            var authorVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return authorVM;

        }

        public Response getById(Guid id)
        {
            var author = _context.Authors.FirstOrDefault((author
                => author.Id == id));

            return new Response
            {
                resultCd = 0,
                Data = author
            };
        }

        public Response getByName(String name)
        {
            var author = _context.Authors.FirstOrDefault((author
                => author.Name == name));

            return new Response
            {
                resultCd = 0,
                Data = author
            };
        }

        public Response Update(AuthorModel authorModel)
        {
            var author = _context.Authors.FirstOrDefault((author
                => author.Id == authorModel.Id));
            if (author != null)
            {
                author.UpdateDate = DateTime.UtcNow;
                author.Name = authorModel.Name;
                author.PhoneNumber = authorModel.PhoneNumber;
                author.Email = authorModel.Email;
                author.Address = authorModel.Address;
                author.Description = authorModel.Description;
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I402",
                // Update type success               
            };
        }
    }
}
