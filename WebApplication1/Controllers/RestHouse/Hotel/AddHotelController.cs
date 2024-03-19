using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;
using WebApplication1.Model.RestHouse;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.RestHouse.Hotel
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddHotelController : ControllerBase
    {
        private readonly HotelService _entityService;

        public AddHotelController(HotelService airPlaneService)
        {
            _entityService = airPlaneService;
        }

        [HttpPost]
        [Authorize(Policy = "CreateHotelAccess")]
        public IActionResult CreateNew(HotelModel entityModel)
        {
            return Ok(_entityService.CreateNew(entityModel));
        }
    }
}
