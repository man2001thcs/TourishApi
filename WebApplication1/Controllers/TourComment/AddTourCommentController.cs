using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.TourComment
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddTourCommentController : ControllerBase
    {
        private readonly ITourishCommentRepository _entityRepository;

        public AddTourCommentController(ITourishCommentRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpPost]
        [Authorize(Policy = "CreateTourishCommentAccess")]
        public IActionResult CreateNew(TourishCommentModel entityModel)
        {
            try
            {
                var response = _entityRepository.Add(entityModel);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
