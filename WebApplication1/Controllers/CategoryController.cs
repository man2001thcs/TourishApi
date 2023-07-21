using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var bookList = _categoryRepository.GetAll();
                return Ok(bookList);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid Id)
        {
            try
            {
                var category = _categoryRepository.getById(Id);
                if (category == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(category);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpPost]
        public IActionResult CreateNew(CategoryModel categoryModel)
        {
            try
            {
                var category = new CategoryModel
                {
                    Id = categoryModel.Id,
                    Name = categoryModel.Name,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,

                };
                _categoryRepository.Add(category);
                return Ok(category);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategoryById(Guid Id, CategoryVM categoryVM)
        {
            try
            {
                _categoryRepository.Update(categoryVM);
                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
