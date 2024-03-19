using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Transport.AirPlane
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteAirPlaneController : ControllerBase
    {
        private readonly AirPlaneService _entityService;

        public DeleteAirPlaneController(AirPlaneService entityService)
        {
            _entityService = entityService;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteAirPlaneAccess")]
        public IActionResult DeleteById(Guid id)
        {
            return Ok(_entityService.DeleteById(id));
        }
    }
}
