using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface.Receipt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Receipt
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetFullReceiptController : ControllerBase
    {
        private readonly IReceiptRepository _receiptRepository;

        public GetFullReceiptController(IReceiptRepository receiptRepository)
        {
            _receiptRepository = receiptRepository;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var receipt = _receiptRepository.getFullReceiptById(id);
                if (receipt.Data == null)
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C510",
                    };
                    return NotFound(response);
                }
                else
                {
                    return Ok(receipt);
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}