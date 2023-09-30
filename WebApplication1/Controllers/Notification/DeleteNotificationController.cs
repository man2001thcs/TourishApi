using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteNotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public DeleteNotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteNotificationAccess")]
        public IActionResult DeleteById(Guid id)
        {

            try
            {
                _notificationRepository.Delete(id);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I703",
                };
                return Ok(response);
            }
            catch
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C704",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }


        }
    }
}
