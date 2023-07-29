using Microsoft.AspNetCore.Mvc;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Category
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetCategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? search, string? sortBy, int page = 1)
        {
            try
            {
                var categoryList = _categoryRepository.GetAll(search, sortBy, page = 1);
                return Ok(categoryList);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var category = _categoryRepository.getById(id);
            if (category == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(category);
            }
        }
    }
}

