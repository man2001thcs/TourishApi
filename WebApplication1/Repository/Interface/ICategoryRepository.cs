using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface
{
    public interface ICategoryRepository
    {
        List<CategoryVM> GetAll();
        CategoryVM getById(Guid id);
        CategoryVM Add(CategoryModel categoryModel);
        void Update(CategoryVM categoryVM);
        void Delete(Guid id);
    }
}
