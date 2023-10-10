using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.TourishPlan
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteTourishPlanController : ControllerBase
    {
        private readonly ITourishPlanRepository _entityRepository;

        public DeleteTourishPlanController(ITourishPlanRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteTourishPlanAccess")]
        public IActionResult DeleteById(Guid id)
        {
            {
                try
                {
                    _entityRepository.Delete(id);
                    var response = new Response
                    {
                        resultCd = 0,
                        MessageCode = "I413",
                    };
                    return Ok(response);
                }
                catch
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C414",
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

            }
        }
    }
}
