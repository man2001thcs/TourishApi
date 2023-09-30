using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Message
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteMessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public DeleteMessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteMessageAccess")]
        public IActionResult DeleteById(Guid id)
        {

            try
            {
                _messageRepository.Delete(id);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I603",
                };
                return Ok(response);
            }
            catch
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C604",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }


        }
    }
}
