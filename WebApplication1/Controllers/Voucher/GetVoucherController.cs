using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Voucher
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetVoucherController : ControllerBase
    {
        private readonly IVoucherRepository _voucherRepository;

        public GetVoucherController(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            try
            {
                var voucherList = _voucherRepository.GetAll(search, sortBy, page, pageSize);
                return Ok(voucherList);
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

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var voucher = _voucherRepository.getById(id);
                if (voucher.Data == null)
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C300",
                    };
                    return NotFound(response);
                }
                else
                {
                    return Ok(voucher);
                }
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

