using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Category
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteCategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public DeleteCategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository =categoryRepository;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteCategoryAccess")]
        public IActionResult DeleteById(Guid id)
        {
            {
                try
                {
                    _categoryRepository.Delete(id);
                    return NoContent();
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

            }
        }
    }
}
