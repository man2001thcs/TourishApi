using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TourishApi.Service.InheritanceService.Schedule;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Schedule
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetMovingScheduleController : ControllerBase
    {
        private readonly MovingScheduleService _entityService;

        public GetMovingScheduleController(MovingScheduleService entityService)
        {
            _entityService = entityService;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(
            string? search,
            int? type,
             double? priceFrom,
            double? priceTo,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 5
        )
        {
            return Ok(
                _entityService.GetAll(search, type, priceFrom, priceTo, sortBy, sortDirection, "", page, pageSize)
            );
        }

        [HttpGet("with-authority")]
        [Authorize]
        public IActionResult GetAllWithAuthority(
            string? search,
            int? type,
             double? priceFrom,
            double? priceTo,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 5
        )
        {
            string userId = User.FindFirstValue("Id");
            return Ok(
                _entityService.GetAll(search, type, priceFrom, priceTo, sortBy, sortDirection, userId, page, pageSize)
            );
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(_entityService.GetById(id));
        }

        [HttpGet("client/{id}")]
        public IActionResult clientGetById(Guid id)
        {
            return Ok(_entityService.clientGetById(id));
        }

        [HttpGet("interest")]
        public IActionResult GetById(Guid scheduleId, Guid userId)
        {
            return Ok(_entityService.getScheduleInterest(scheduleId, userId));
        }
    }
}
