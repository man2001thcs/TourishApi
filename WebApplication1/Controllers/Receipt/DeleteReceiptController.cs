﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface.Receipt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Receipt
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteReceiptController : ControllerBase
    {
        private readonly IReceiptRepository _receiptRepository;

        public DeleteReceiptController(IReceiptRepository receiptRepository)
        {
            _receiptRepository = receiptRepository;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteReceiptAccess")]
        public IActionResult DeleteById(Guid id)
        {

            try
            {
                _receiptRepository.Delete(id);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I513",
                };
                return Ok(response);
            }
            catch
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }


        }
    }
}
