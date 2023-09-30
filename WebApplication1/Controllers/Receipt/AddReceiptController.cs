using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Receipt
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddReceiptController : ControllerBase
    {
        private readonly IReceiptRepository _receiptRepository;

        private readonly char[] delimiter = new char[] { ';' };

        public AddReceiptController(IReceiptRepository receiptRepository
            )
        {
            _receiptRepository = receiptRepository;
        }

        [HttpPost]
        [Authorize(Policy = "CreateReceiptAccess")]
        public async Task<IActionResult> CreateNew(ReceiptInsertModel receiptInsertModel)
        {
            try
            {

                var receiptReturn = await _receiptRepository.Add(receiptInsertModel);

                var receiptReturnId = (Guid)receiptReturn.returnId;

                Debug.Write(receiptReturnId);

                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I801",
                };
                return Ok(response);


            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C805",
                    Error = ex.Message,
                    Data = ex

                };
                return BadRequest(response);
            }
        }
    }
}
