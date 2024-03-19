using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;
using WebApplication1.Model.RestHouse;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.RestHouse.Hotel
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateHotelController : ControllerBase
    {
        private readonly HotelService _entityService;

        public UpdateHotelController(HotelService entityService)
        {
            _entityService = entityService;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateHotelAccess")]
        public IActionResult UpdateHotelById(Guid id, HotelModel HotelModel)
        {
            return Ok(_entityService.UpdateEntityById(id, HotelModel));
        }
    }
}
