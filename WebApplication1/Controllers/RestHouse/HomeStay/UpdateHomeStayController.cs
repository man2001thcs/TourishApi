using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;
using WebApplication1.Model.RestHouse;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.RestHouse.HomeStay
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateHomeStayController : ControllerBase
    {
        private readonly HomeStayService _entityService;

        public UpdateHomeStayController(HomeStayService entityService)
        {
            _entityService = entityService;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateHomeStayAccess")]
        public IActionResult UpdateHomeStayById(Guid id, HomeStayModel HomeStayModel)
        {
            return Ok(_entityService.UpdateEntityById(id, HomeStayModel));
        }
    }
}
