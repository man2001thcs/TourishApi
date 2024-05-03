﻿using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService.Schedule;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Schedule
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetStayingScheduleController : ControllerBase
    {
        private readonly StayingScheduleService _entityService;

        public GetStayingScheduleController(StayingScheduleService entityService)
        {
            _entityService = entityService;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? search, int? type, string? sortBy, int page = 1, int pageSize = 5)
        {
            return Ok(_entityService.GetAll(search, type, sortBy, page, pageSize));

        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(_entityService.GetById(id));
        }

        [HttpGet("interest")]
        public IActionResult GetById(Guid scheduleId, Guid userId)
        {
            return Ok(_entityService.getScheduleInterest(scheduleId,
                userId));
        }
    }
}

