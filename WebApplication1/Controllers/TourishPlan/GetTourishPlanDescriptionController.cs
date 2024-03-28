using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.TourishPlan
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetTourishPlanDescriptionController : ControllerBase
    {
        private readonly TourishPlanService _entityService;

        public GetTourishPlanDescriptionController(TourishPlanService entityService)
        {
            _entityService = entityService;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<IActionResult> GetDescription(string containerName, string blobName)
        {
            return Ok(await _entityService.getDescription(containerName, blobName));
        }
    }
}



