﻿using Microsoft.AspNetCore.Authorization;
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
    public class AddMovingScheduleController : ControllerBase
    {
        private readonly MovingScheduleService _entityService;

        public AddMovingScheduleController(MovingScheduleService airPlaneService)
        {
            _entityService = airPlaneService;
        }

        [HttpPost]
        [Authorize(Policy = "CreateTourishPlanAccess")]
        public async Task<IActionResult> CreateNew(MovingScheduleModel entityModel)
        {
            string userId = User.FindFirstValue("Id");
            return Ok(await _entityService.CreateNew(userId, entityModel));
        }

        [HttpPost("interest")]
        [Authorize]
        public async Task<IActionResult> SetInterest(ScheduleInterestModel tourishInterestModel)
        {
            var response = await _entityService.setScheduleInterest(tourishInterestModel.ScheduleId,
                tourishInterestModel.UserId, tourishInterestModel.InterestStatus);
            return Ok(response);
        }
    }
}
