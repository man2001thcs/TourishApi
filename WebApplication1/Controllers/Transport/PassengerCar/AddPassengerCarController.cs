using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TourishApi.Repository.Interface.Schedule;
using TourishApi.Repository.Interface.Transport;
using WebApplication1.Model.Transport;
using WebApplication1.Model.VirtualModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Transport.PassengerCar
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddPassengerCarController : ControllerBase
    {
        private readonly IPassengerCarRepository _entityRepository;

        public AddPassengerCarController(IPassengerCarRepository airPlaneRepository)
        {
            _entityRepository = airPlaneRepository;
        }

        [HttpPost]
        [Authorize(Policy = "CreatePassengerCarAccess")]
        public IActionResult CreateNew(PassengerCarModel entityModel)
        {
            try
            {
                var entityExist = _entityRepository.getByName(entityModel.BranchName);

                if (entityExist.Data == null)
                {
                    var response = _entityRepository.Add(entityModel);

                    return Ok(response);
                }
                else
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C111",
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
