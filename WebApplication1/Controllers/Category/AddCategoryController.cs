using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Category
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddCategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public AddCategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpPost]
        [Authorize(Policy = "CreateCategoryAccess")]
        public IActionResult CreateNew(CategoryModel categoryModel)
        {
            try
            {
                var category = new CategoryModel
                {
                    Name = categoryModel.Name,
                    Description = categoryModel.Description,
                };
                _categoryRepository.Add(category);
                return StatusCode(StatusCodes.Status201Created, category);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
