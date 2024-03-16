using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.TourCategory
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteTourCategoryController : ControllerBase
    {
        private readonly ITourishCategoryRepository _entityRepository;

        public DeleteTourCategoryController(ITourishCategoryRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteTourishCategoryAccess")]
        public IActionResult DeleteById(Guid id)
        {
            {
                try
                {
                    _entityRepository.Delete(id);
                    var response = new Response
                    {
                        resultCd = 0,
                        MessageCode = "I423",
                    };
                    return Ok(response);
                }
                catch
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C424",
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

            }
        }
    }
}
