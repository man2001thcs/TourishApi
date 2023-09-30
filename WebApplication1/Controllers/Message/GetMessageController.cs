using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Message
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetMessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public GetMessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? search, Guid UserId, string? sortBy, int page = 1, int pageSize = 5)
        {
            try
            {
                var messageList = _messageRepository.GetAll(search, UserId, sortBy, page, pageSize);
                return Ok(messageList);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C604",
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
                var message = _messageRepository.getById(id);
                if (message.Data == null)
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C600",
                    };
                    return NotFound(response);
                }
                else
                {
                    return Ok(message);
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C604",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


    }
}

