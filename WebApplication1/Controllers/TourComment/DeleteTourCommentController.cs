using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.TourComment
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteTourCommentController : ControllerBase
    {
        private readonly ITourishCommentRepository _entityRepository;

        public DeleteTourCommentController(ITourishCommentRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteTourishCommentAccess")]
        public IActionResult DeleteById(Guid id)
        {
            {
                try
                {
                    _entityRepository.Delete(id);
                    var response = new Response
                    {
                        resultCd = 0,
                        MessageCode = "I613",
                    };
                    return Ok(response);
                }
                catch
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C614",
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

            }
        }
    }
}
