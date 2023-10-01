using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Repository.Interface.Schedule;
using TourishApi.Repository.Interface.Transport;
using WebApplication1.Model.VirtualModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Transport.PassengerCar
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeletePassengerCarController : ControllerBase
    {
        private readonly IPassengerCarRepository _entityRepository;

        public DeletePassengerCarController(IPassengerCarRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeletePassengerCarAccess")]
        public IActionResult DeleteById(Guid id)
        {
            {
                try
                {
                    _entityRepository.Delete(id);
                    var response = new Response
                    {
                        resultCd = 0,
                        MessageCode = "I123",
                    };
                    return Ok(response);
                }
                catch
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C124",
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

            }
        }
    }
}
