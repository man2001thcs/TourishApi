using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hub;
using SignalR.Hub.Client;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddNotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private IHubContext<NotificationHub, INotificationHubClient> notificationHub;

        public AddNotificationController(INotificationRepository notificationRepository, IHubContext<NotificationHub,
            INotificationHubClient> _notificationHub)
        {
            _notificationRepository = notificationRepository;
            notificationHub = _notificationHub;
        }

        [HttpPost]
        [Authorize(Policy = "CreateNotificationAccess")]
        public IActionResult CreateNew(NotificationModel notificationModel)
        {
            try
            {
                var notificationExist = _notificationRepository.getByName(notificationModel.Content);

                if (notificationExist.Data == null)
                {
                    var notification = new NotificationModel
                    {
                        UserId = notificationModel.UserId,
                        Content = notificationModel.Content,
                        IsRead = notificationModel.IsRead,
                        IsDeleted = notificationModel.IsDeleted,
                        UpdateDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                    };

                    _notificationRepository.Add(notification);

                    notificationHub.Clients.All.SendOffersToAll(notification);

                    var response = new Response
                    {
                        resultCd = 0,
                        MessageCode = "I701",
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C701",
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
