using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Transport.PassengerCar
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeletePassengerCarController : ControllerBase
    {
        private readonly PassengerCarService _entityService;

        public DeletePassengerCarController(PassengerCarService entityService)
        {
            _entityService = entityService;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeletePassengerCarAccess")]
        public IActionResult DeleteById(Guid id)
        {
            return Ok(_entityService.DeleteById(id));
        }
    }
}
