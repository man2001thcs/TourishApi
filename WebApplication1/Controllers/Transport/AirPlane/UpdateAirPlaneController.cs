using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Repository.Interface.Transport;
using WebApplication1.Model.Transport;
using WebApplication1.Model.VirtualModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.AirPlane
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateAirPlaneController : ControllerBase
    {
        private readonly IAirPlaneRepository _entityRepository;

        public UpdateAirPlaneController(IAirPlaneRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateAirPlaneAccess")]
        public IActionResult UpdateAirPlaneById(Guid id, AirPlaneModel AirPlaneModel)
        {

            try
            {
                var response = _entityRepository.Update(AirPlaneModel);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C124",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }



        }
    }
}
