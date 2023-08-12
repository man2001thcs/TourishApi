using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Publisher
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetPublisherController : ControllerBase
    {
        private readonly IPublisherRepository _publisherRepository;

        public GetPublisherController(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            try
            {
                var publisherList = _publisherRepository.GetAll(search, sortBy, page, pageSize);
                return Ok(publisherList);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C504",
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
                var publisher = _publisherRepository.getById(id);
                if (publisher.Data == null)
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C500",
                    };
                    return NotFound(response);
                }
                else
                {
                    return Ok(publisher);
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C504",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}

