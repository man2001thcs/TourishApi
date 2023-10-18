using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.Receipt;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface.Receipt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Receipt
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetReceiptController : ControllerBase
    {
        private readonly IReceiptRepository _receiptRepository;

        public GetReceiptController(IReceiptRepository receiptRepository)
        {
            _receiptRepository = receiptRepository;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? tourishPlanId, string? sortBy, int page = 1, int pageSize = 5, ReceiptStatus status = ReceiptStatus.Created)
        {
            try
            {
                var receiptList = _receiptRepository.GetAll(tourishPlanId, status, sortBy, page, pageSize);
                return Ok(receiptList);
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

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var receipt = _receiptRepository.getById(id);
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