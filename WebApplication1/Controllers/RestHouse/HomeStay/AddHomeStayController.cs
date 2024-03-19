using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;
using WebApplication1.Model.RestHouse;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.RestHouse.HomeStay
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddHomeStayController : ControllerBase
    {
        private readonly HomeStayService _entityService;

        public AddHomeStayController(HomeStayService airPlaneService)
        {
            _entityService = airPlaneService;
        }

        [HttpPost]
        [Authorize(Policy = "CreateHomeStayAccess")]
        public IActionResult CreateNew(HomeStayModel entityModel)
        {
            return Ok(_entityService.CreateNew(entityModel));
        }
    }
}
