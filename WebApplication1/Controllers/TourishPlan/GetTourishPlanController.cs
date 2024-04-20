using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.TourishPlan
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetTourishPlanController : ControllerBase
    {
        private readonly TourishPlanService _entityService;

        public GetTourishPlanController(TourishPlanService entityService)
        {
            _entityService = entityService;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? search, string? category, string? startingPoint, string? endPoint, string? startingDate,
            double? priceFrom, double? priceTo, string? sortBy, int page = 1, int pageSize = 5)
        {
            return Ok(_entityService.GetAll(search, category, startingPoint, endPoint, startingDate, priceFrom, priceTo, sortBy, page, pageSize));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(_entityService.GetById(id));
        }
    }
}