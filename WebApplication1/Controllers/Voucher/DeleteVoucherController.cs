using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Voucher
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteVoucherController : ControllerBase
    {
        private readonly IVoucherRepository _voucherRepository;

        public DeleteVoucherController(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteVoucherAccess")]
        public IActionResult DeleteById(Guid id)
        {

            try
            {
                _voucherRepository.Delete(id);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I303",
                };
                return Ok(response);
            }
            catch
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C304",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }


        }
    }
}
