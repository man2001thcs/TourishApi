using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TourishApi.Service.InheritanceService.Schedule;
using WebApplication1.Model;
using WebApplication1.Model.Schedule;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Schedule
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateStayingScheduleController : ControllerBase
    {
        private readonly StayingScheduleService _entityService;

        public UpdateStayingScheduleController(StayingScheduleService entityService)
        {
            _entityService = entityService;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateTourishPlanAccess")]
        public async Task<IActionResult> UpdateStayingScheduleById(Guid id, StayingScheduleModel StayingScheduleModel)
        {
            string userId = User.FindFirstValue("Id");
            return Ok(await _entityService.UpdateEntityById(userId, StayingScheduleModel));
        }

        [HttpPut("interest")]
        public async Task<IActionResult> SetInterest(TourishInterestModel tourishInterestModel)
        {
            var response = await _entityService.setScheduleInterest(tourishInterestModel.TourishPlanId,
                tourishInterestModel.UserId, tourishInterestModel.InterestStatus);
            return Ok(response);
        }

        [HttpPut("instruction")]
        public async Task<IActionResult> SetInstruction(ScheduleInstructionModel model)
        {
            var response = await _entityService.UpdateInstructionList(model);
            return Ok(response);
        }
    }
}
