using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetNotificationController : ControllerBase
    {
        private readonly NotificationService _entityService;

        public GetNotificationController(NotificationService entityService)
        {
            _entityService = entityService;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? search, int? type, string? sortBy, int page = 1, int pageSize = 5)
        {
            return Ok(_entityService.GetAll(search, type, sortBy, page, pageSize));

        }

        [HttpGet("creator")]
        public IActionResult GetForCreator(string? creatorId, string? sortBy, int page = 1, int pageSize = 5)
        {
            return Ok(_entityService.GetAllForCreator(creatorId, sortBy, page, pageSize));

        }

        [HttpGet("receiver")]
        public IActionResult GetForReceiver(string? receiverId, string? sortBy, int page = 1, int pageSize = 5)
        {
            return Ok(_entityService.GetAllForReceiver(receiverId, sortBy, page, pageSize));

        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(_entityService.GetById(id));
        }
    }
}

