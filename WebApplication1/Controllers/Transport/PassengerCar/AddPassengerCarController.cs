using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;
using WebApplication1.Model.Transport;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Transport.PassengerCar
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddPassengerCarController : ControllerBase
    {
        private readonly PassengerCarService _entityService;

        public AddPassengerCarController(PassengerCarService airPlaneService)
        {
            _entityService = airPlaneService;
        }

        [HttpPost]
        [Authorize(Policy = "CreatePassengerCarAccess")]
        public IActionResult CreateNew(PassengerCarModel entityModel)
        {
            return Ok(_entityService.CreateNew(entityModel));
        }
    }
}
