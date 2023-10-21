using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.Receipt;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface.Receipt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Receipt
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateReceiptController : ControllerBase
    {
        private readonly IReceiptRepository _receiptRepository;

        public UpdateReceiptController(IReceiptRepository receiptRepository)
        {
            _receiptRepository = receiptRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateReceiptAccess")]
        public async Task<IActionResult> UpdateReceiptById(Guid fullReceiptId, FullReceiptUpdateModel receiptModel)
        {
            try
            {
                await _receiptRepository.Update(receiptModel);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I512",
                };
                return Ok(response);
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
