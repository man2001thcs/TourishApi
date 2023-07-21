using WebApplication1.Data;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.InheritanceRepo
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MyDbContext _context;
        public CategoryRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public CategoryVM Add(CategoryModel categoryModel)
        {

            var category = new Category
            {
                Id = categoryModel.Id,
                Name = categoryModel.Name,
            };
            _context.Add(category);
            _context.SaveChanges();

            return new CategoryVM
            {
                Id = categoryModel.Id,
                Name = categoryModel.Name
            ,
                CreateDate = category.CreateDate,
                UpdateDate = category.UpdateDate
            };

        }

        public void Delete(Guid id)
        {
            var category = _context.Categories.FirstOrDefault((category
               => category.Id == id));
            if (category != null)
            {
                _context.Remove(category);
                _context.SaveChanges();
            }
        }

        public List<CategoryVM> GetAll()
        {
            var bookList = _context.Categories.Select(category => new CategoryVM
            {
                Id = category.Id,
                Name = category.Name,
                CreateDate = category.CreateDate,
                UpdateDate = category.UpdateDate
            });
            return bookList.ToList();

        }

        public CategoryVM getById(Guid id)
        {
            var category = _context.Categories.FirstOrDefault((category
                => category.Id == id));
            if (category == null) { return null; }
            return new CategoryVM
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public void Update(CategoryVM categoryVM)
        {
            var category = _context.Categories.FirstOrDefault((category
                => category.Id == categoryVM.Id));
            if (category != null)
            {
                category.UpdateDate = DateTime.UtcNow;
                category.Name = categoryVM.Name;
                _context.SaveChanges();
            }
        }
    }
}
