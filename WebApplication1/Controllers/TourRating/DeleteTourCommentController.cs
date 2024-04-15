using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.TourRating
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteTourRatingController : ControllerBase
    {
        private readonly TourishCommentService _entityService;

        public DeleteTourRatingController(TourishCommentService entityService)
        {
            _entityService = entityService;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteTourishCommentAccess")]
        public IActionResult DeleteById(Guid id)
        {
            {
                return Ok(_entityService.DeleteById(id));
            }
        }
    }
}
