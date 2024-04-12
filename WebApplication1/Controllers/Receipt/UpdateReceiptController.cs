using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;
using WebApplication1.Model.Receipt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Receipt
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateReceiptController : ControllerBase
    {
        private readonly ReceiptService _receiptService;

        private readonly char[] delimiter = new char[] { ';' };

        public UpdateReceiptController(ReceiptService receiptService
            )
        {
            _receiptService = receiptService;
        }


        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateReceiptAccess")]
        public async Task<IActionResult> UpdateReceiptById(Guid fullReceiptId, FullReceiptUpdateModel receiptModel)
        {
            return Ok(_receiptService.UpdateReceiptById(fullReceiptId, receiptModel));
        }
    }
}
