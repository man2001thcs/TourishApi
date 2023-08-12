using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Voucher
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateVoucherController : ControllerBase
    {
        private readonly IVoucherRepository _voucherRepository;

        public UpdateVoucherController(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateVoucherAccess")]
        public IActionResult UpdateVoucherById(Guid id, VoucherModel VoucherModel)
        {

            try
            {
                _voucherRepository.Update(VoucherModel);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I302",
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C304",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
