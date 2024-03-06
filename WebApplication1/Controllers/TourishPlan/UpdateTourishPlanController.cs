using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.TourishPlan
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateTourishPlanController : ControllerBase
    {
        private readonly ITourishPlanRepository _entityRepository;

        public UpdateTourishPlanController(ITourishPlanRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateTourishPlanAccess")]
        public async Task<IActionResult> UpdateTourishPlanById(TourishPlanUpdateModel entityModel)
        {

            try
            {
                string userId = User.FindFirstValue("Id");
                var passAwayEntity = entityModel;
                passAwayEntity.ModifierId = new Guid(userId);
                var response = await _entityRepository.Update(passAwayEntity);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C414",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }



        }
    }
}
