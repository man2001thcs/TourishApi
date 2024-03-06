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
    public class AddTourishPlanController : ControllerBase
    {
        private readonly ITourishPlanRepository _entityRepository;

        public AddTourishPlanController(ITourishPlanRepository airPlaneRepository)
        {
            _entityRepository = airPlaneRepository;
        }

        [HttpPost]
        [Authorize(Policy = "CreateTourishPlanAccess")]
        public async Task<IActionResult> CreateNew(TourishPlanInsertModel entityModel)
        {
            try
            {
                var entityExist = _entityRepository.getByName(entityModel.TourName);


                // Lấy ID từ token                    
                // Tiếp tục xử lý logic của bạn ở đây với userId đã lấy được
                if (entityExist.Data == null)
                {
                    string userId = User.FindFirstValue("Id");
                    var passAwayEntity = entityModel;
                    passAwayEntity.CreatorId = new Guid(userId);
                    var response = await _entityRepository.Add(passAwayEntity);

                    return Ok(response);
                }
                else
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C411",
                    };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
