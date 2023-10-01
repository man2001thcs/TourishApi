using Microsoft.AspNetCore.Mvc;
using TourishApi.Repository.Interface.Resthouse;
using WebApplication1.Model.VirtualModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.RestHouse.HomeStay
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetPassengerCarController : ControllerBase
    {
        private readonly IHotelRepository _entityRepository;

        public GetPassengerCarController(IHotelRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            try
            {
                var authorList = _entityRepository.GetAll(search, sortBy, page, pageSize);
                return Ok(authorList);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C224",
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
                var author = _entityRepository.getById(id);
                if (author.Data == null)
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C220",
                    };
                    return NotFound(response);
                }
                else
                {
                    return Ok(author);
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C224",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}

