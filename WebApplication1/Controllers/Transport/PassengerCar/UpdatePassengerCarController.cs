using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;
using WebApplication1.Model.Transport;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Transport.PassengerCar
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdatePassengerCarController : ControllerBase
    {
        private readonly PassengerCarService _entityService;

        public UpdatePassengerCarController(PassengerCarService entityService)
        {
            _entityService = entityService;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdatePassengerCarAccess")]
        public IActionResult UpdatePassengerCarById(Guid id, PassengerCarModel PassengerCarModel)
        {
            return Ok(_entityService.UpdateEntityById(id, PassengerCarModel));
        }
    }
}
