using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService.Schedule;
using WebApplication1.Model.Schedule;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Schedule
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddMovingScheduleController : ControllerBase
    {
        private readonly MovingScheduleService _entityService;

        public AddMovingScheduleController(MovingScheduleService airPlaneService)
        {
            _entityService = airPlaneService;
        }

        [HttpPost]
        [Authorize(Policy = "CreateTourishPlanAccess")]
        public IActionResult CreateNew(MovingScheduleModel entityModel)
        {
            return Ok(_entityService.CreateNew(entityModel));
        }
    }
}
