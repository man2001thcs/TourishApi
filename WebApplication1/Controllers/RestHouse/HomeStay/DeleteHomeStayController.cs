using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Repository.Interface.Resthouse;
using WebApplication1.Model.VirtualModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.RestHouse.HomeStay
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteHomeStayController : ControllerBase
    {
        private readonly IHomeStayRepository _entityRepository;

        public DeleteHomeStayController(IHomeStayRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteHomeStayAccess")]
        public IActionResult DeleteById(Guid id)
        {
            {
                try
                {
                    _entityRepository.Delete(id);
                    var response = new Response
                    {
                        resultCd = 0,
                        MessageCode = "I223",
                    };
                    return Ok(response);
                }
                catch
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C224",
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

            }
        }
    }
}
