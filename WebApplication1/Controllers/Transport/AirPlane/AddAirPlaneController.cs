using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;
using WebApplication1.Model.Transport;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Transport.AirPlane
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddAirPlaneController : ControllerBase
    {
        private readonly AirPlaneService _entityService;

        public AddAirPlaneController(AirPlaneService airPlaneService)
        {
            _entityService = airPlaneService;
        }

        [HttpPost]
        [Authorize(Policy = "CreateAirPlaneAccess")]
        public IActionResult CreateNew(AirPlaneModel entityModel)
        {
            return Ok(_entityService.CreateNew(entityModel));
        }
    }
}
