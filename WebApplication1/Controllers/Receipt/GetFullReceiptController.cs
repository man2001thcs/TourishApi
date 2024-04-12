using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Receipt
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetFullReceiptController : ControllerBase
    {
        private readonly ReceiptService _receiptService;

        private readonly char[] delimiter = new char[] { ';' };

        public GetFullReceiptController(ReceiptService receiptService
            )
        {
            _receiptService = receiptService;
        }


        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(_receiptService.GetFullReceiptById(id));
        }
    }
}