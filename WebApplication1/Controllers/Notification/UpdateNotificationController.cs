using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateNotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public UpdateNotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateNotificationAccess")]
        public IActionResult UpdateNotificationById(Guid id, NotificationModel NotificationModel)
        {

            try
            {
                _notificationRepository.Update(NotificationModel);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I702",
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C704",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }



        }
    }
}
