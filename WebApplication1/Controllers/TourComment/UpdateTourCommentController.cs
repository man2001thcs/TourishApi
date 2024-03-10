using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.TourComment
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateTourCommentController : ControllerBase
    {
        private readonly ITourishCommentRepository _entityRepository;

        public UpdateTourCommentController(ITourishCommentRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateTourishCommentAccess")]
        public IActionResult UpdateTourishCommentById(Guid id, TourishCommentModel TourishCommentModel)
        {

            try
            {
                var response = _entityRepository.Update(TourishCommentModel);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C614",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }



        }
    }
}
