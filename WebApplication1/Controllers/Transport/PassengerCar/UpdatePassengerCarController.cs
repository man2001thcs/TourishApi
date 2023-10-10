using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Repository.Interface.Transport;
using WebApplication1.Model.Transport;
using WebApplication1.Model.VirtualModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Transport.PassengerCar
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdatePassengerCarController : ControllerBase
    {
        private readonly IPassengerCarRepository _entityRepository;

        public UpdatePassengerCarController(IPassengerCarRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdatePassengerCarAccess")]
        public IActionResult UpdatePassengerCarById(Guid id, PassengerCarModel PassengerCarModel)
        {

            try
            {
                var response = _entityRepository.Update(PassengerCarModel);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C114",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }



        }
    }
}
