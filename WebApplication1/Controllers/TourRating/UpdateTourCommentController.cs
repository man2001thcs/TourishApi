using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;
using WebApplication1.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.TourRating
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateTourRatingController : ControllerBase
    {
        private readonly TourishCommentService _entityService;

        public UpdateTourRatingController(TourishCommentService entityService)
        {
            _entityService = entityService;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateTourishCommentAccess")]
        public IActionResult UpdateTourishCommentById(Guid id, TourishCommentModel TourishCommentModel)
        {
            return Ok(_entityService.UpdateEntityById(id, TourishCommentModel));
        }
    }
}
