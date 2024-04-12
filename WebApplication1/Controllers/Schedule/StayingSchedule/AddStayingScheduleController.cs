﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;
using TourishApi.Service.InheritanceService.Schedule;
using WebApplication1.Model.RestHouse;
using WebApplication1.Model.Schedule;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Schedule
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddStayingScheduleController : ControllerBase
    {
        private readonly StayingScheduleService _entityService;

        public AddStayingScheduleController(StayingScheduleService airPlaneService)
        {
            _entityService = airPlaneService;
        }

        [HttpPost]
        [Authorize(Policy = "CreateStayingScheduleAccess")]
        public IActionResult CreateNew(StayingScheduleModel entityModel)
        {
            return Ok(_entityService.CreateNew(entityModel));
        }
    }
}
