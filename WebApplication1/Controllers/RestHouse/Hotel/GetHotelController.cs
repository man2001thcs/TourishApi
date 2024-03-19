using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Transport.RestHouse.Hotel
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetHotelController : ControllerBase
    {
        private readonly HotelService _entityService;

        public GetHotelController(HotelService entityService)
        {
            _entityService = entityService;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            return Ok(_entityService.GetAll(search, sortBy, page, pageSize));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(_entityService.GetById(id));
        }
    }
}

