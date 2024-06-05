using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        public IActionResult GetAll(string? search, string? category, string? categoryString, string? startingPoint, string? endPoint, string? startingDate,
            double? priceFrom, double? priceTo, string? sortBy, string? sortDirection, int page = 1, int pageSize = 5)
        {
            return Ok(_entityService.GetAll(search, category, categoryString, startingPoint, endPoint, startingDate, priceFrom, priceTo, sortBy, sortDirection, "", page, pageSize));
        }

        [HttpGet("with-authority")]
        [Authorize]
        public IActionResult GetAllWithAuthority(string? search, string? category, string? categoryString, string? startingPoint, string? endPoint, string? startingDate,
            double? priceFrom, double? priceTo, string? sortBy, string? sortDirection, int page = 1, int pageSize = 5)
        {
            string userId = User.FindFirstValue("Id");
            return Ok(_entityService.GetAll(search, category, categoryString, startingPoint, endPoint, startingDate, priceFrom, priceTo, sortBy, sortDirection, userId, page, pageSize));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(_entityService.GetById(id));
        }

        [HttpGet("client/{id}")]
        public IActionResult ClientGetById(Guid id)
        {
            return Ok(_entityService.clientGetById(id));
        }

        [HttpGet("interest")]
        public IActionResult GetById(Guid tourishPlanId, Guid userId)
        {
            return Ok(_entityService.getTourInterest(tourishPlanId,
                userId));
        }

        [HttpGet("top-rating")]
        public IActionResult GetTopTourRating()
        {
            return Ok(_entityService.getTopTourRating());
        }
    }
}