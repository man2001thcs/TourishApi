using Microsoft.AspNetCore.Mvc;
using TourishApi.Repository.Interface.Schedule;
using TourishApi.Repository.Interface.Transport;
using WebApplication1.Model.VirtualModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Transport.AirPlane
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetPassengerCarController : ControllerBase
    {
        private readonly IAirPlaneRepository _entityRepository;

        public GetPassengerCarController(IAirPlaneRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            try
            {
                var entityList = _entityRepository.GetAll(search, sortBy, page, pageSize);
                return Ok(entityList);
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

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var entity = _entityRepository.getById(id);
                if (entity.Data == null)
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C120",
                    };
                    return NotFound(response);
                }
                else
                {
                    return Ok(entity);
                }
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

