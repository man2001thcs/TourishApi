using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Message
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateMessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public UpdateMessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateMessageAccess")]
        public IActionResult UpdateMessageById(Guid id, MessageModel MessageModel)
        {

            try
            {
                _messageRepository.Update(MessageModel);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I602",
                };
                return Ok(response);
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
