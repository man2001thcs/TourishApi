using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Connection;
[Route("api/[controller]")]
[ApiController]
public class GetGuestMessageConHistoryController : ControllerBase
{
    private readonly GuestMessageConHistoryService _entityService;

    public GetGuestMessageConHistoryController(GuestMessageConHistoryService entityService)
    {
        _entityService = entityService;
    }

    // GET: api/<ValuesController>
    [HttpGet]
    public IActionResult GetAll(string? search, int? type, string? sortBy, int page = 1, int pageSize = 5)
    {
        return Ok(_entityService.GetAll(search, type, sortBy, page, pageSize));

    }

    // GET: api/<ValuesController>
    [HttpGet("admin")]
    public IActionResult GetAllForAdmin(string? search, int? type, string? sortBy, int page = 1, int pageSize = 5)
    {
        return Ok(_entityService.GetAllForAdmin(search, type, sortBy, page, pageSize));

    }

    // GET: api/<ValuesController>
    [HttpGet("guest-connection")]
    public IActionResult GetByGuestConId(string connectionId)
    {
        return Ok(_entityService.getByGuestConId(connectionId));

    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        return Ok(_entityService.GetById(id));
    }
}


