using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;
using WebApplication1.Model.Transport;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.AirPlane
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateAirPlaneController : ControllerBase
    {
        private readonly AirPlaneService _entityService;

        public UpdateAirPlaneController(AirPlaneService entityService)
        {
            _entityService = entityService;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateAirPlaneAccess")]
        public IActionResult UpdateAirPlaneById(Guid id, AirPlaneModel AirPlaneModel)
        {

            return Ok(_entityService.UpdateEntityById(id, AirPlaneModel));
        }
    }
}
