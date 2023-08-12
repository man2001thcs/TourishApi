using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.InheritanceRepo
{
    public class PublisherRepository : IPublisherRepository
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 5;
        public PublisherRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Add(PublisherModel publisherModel)
        {

            var publisher = new Publisher
            {
                PublisherName = publisherModel.PublisherName,
                PhoneNumber = publisherModel.PhoneNumber,
                Email = publisherModel.Email,
                Address = publisherModel.Address,
                Description = publisherModel.Description,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };
            _context.Add(publisher);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I501",
                // Create type success               
            };

        }

        public Response Delete(Guid id)
        {
            var publisher = _context.Categories.FirstOrDefault((publisher
               => publisher.Id == id));
            if (publisher != null)
            {
                _context.Remove(publisher);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I503",
                // Delete type success               
            };
        }

        public Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            var publisherQuery = _context.Publishers.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                publisherQuery = publisherQuery.Where(publisher => publisher.PublisherName.Contains(search));
            }
            #endregion

            #region Sorting
            //Default sort by Name (TenHh)
            publisherQuery = publisherQuery.OrderBy(publisher => publisher.PublisherName);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        publisherQuery = publisherQuery.OrderByDescending(publisher => publisher.PublisherName);
                        break;
                    case "updateDate_asc":
                        publisherQuery = publisherQuery.OrderBy(publisher => publisher.UpdateDate);
                        break;
                    case "updateDate_desc":
                        publisherQuery = publisherQuery.OrderByDescending(publisher => publisher.UpdateDate);
                        break;
                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<Publisher>.Create(publisherQuery, page, pageSize);
            #endregion

            var publisherVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return publisherVM;

        }

        public Response getById(Guid id)
        {
            var publisher = _context.Publishers.FirstOrDefault((publisher
                => publisher.Id == id));

            return new Response
            {
                resultCd = 0,
                Data = publisher
            };
        }

        public Response getByName(String name)
        {
            var publisher = _context.Publishers.FirstOrDefault((publisher
                => publisher.PublisherName == name));

            return new Response
            {
                resultCd = 0,
                Data = publisher
            };
        }

        public Response Update(PublisherModel publisherModel)
        {
            var publisher = _context.Publishers.FirstOrDefault((publisher
                => publisher.Id == publisherModel.Id));
            if (publisher != null)
            {
                publisher.UpdateDate = DateTime.UtcNow;
                publisher.PublisherName = publisherModel.PublisherName;
                publisher.PhoneNumber = publisherModel.PhoneNumber;
                publisher.Email = publisherModel.Email;
                publisher.Address = publisherModel.Address;
                publisher.Description = publisherModel.Description;
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I502",
                // Update type success               
            };
        }
    }
}
