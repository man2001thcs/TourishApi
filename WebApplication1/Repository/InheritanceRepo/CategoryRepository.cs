using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.InheritanceRepo
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 5;
        public CategoryRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Add(CategoryModel categoryModel)
        {

            var category = new Category
            {
                Id = categoryModel.Id,
                Name = categoryModel.Name,
            };
            _context.Add(category);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I001",
                // Create type success               
            };

        }

        public Response Delete(Guid id)
        {
            var category = _context.Categories.FirstOrDefault((category
               => category.Id == id));
            if (category != null)
            {
                _context.Remove(category);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "D001",
                // Delete type success               
            };
        }

        public Response GetAll(string? search, string? sortBy, int page = 1)
        {
            var categoryQuery = _context.Categories.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                categoryQuery = categoryQuery.Where(category => category.Name.Contains(search));
            }
            #endregion

            #region Sorting
            //Default sort by Name (TenHh)
            categoryQuery = categoryQuery.OrderBy(category => category.Name);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        categoryQuery = categoryQuery.OrderByDescending(category => category.Name);
                        break;
                    case "updateDate_asc":
                        categoryQuery = categoryQuery.OrderBy(category => category.UpdateDate);
                        break;
                    case "updateDate_desc":
                        categoryQuery = categoryQuery.OrderByDescending(category => category.UpdateDate);
                        break;
                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<Category>.Create(categoryQuery, page, PAGE_SIZE);
            #endregion

            var categoryVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
            };
            return categoryVM;

        }

        public Response getById(Guid id)
        {
            var category = _context.Categories.FirstOrDefault((category
                => category.Id == id));
            if (category == null) { return null; }

            return new Response
            {
                resultCd = 0,
                Data = category
            };
        }

        public Response Update(CategoryModel categoryModel)
        {
            var category = _context.Categories.FirstOrDefault((category
                => category.Id == categoryModel.Id));
            if (category != null)
            {
                category.UpdateDate = DateTime.UtcNow;
                category.Name = categoryModel.Name;
                category.Description = categoryModel.Description;
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "U001",
                // Update type success               
            };
        }
    }
}
