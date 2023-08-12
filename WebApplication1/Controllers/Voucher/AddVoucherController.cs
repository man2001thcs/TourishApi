using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Voucher
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddVoucherController : ControllerBase
    {
        private readonly IVoucherRepository _voucherRepository;

        public AddVoucherController(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        [HttpPost]
        [Authorize(Policy = "CreateVoucherAccess")]
        public IActionResult CreateNew(VoucherModel voucherModel)
        {
            try
            {
                var voucherExist = _voucherRepository.getByName(voucherModel.Name);

                if (voucherExist.Data == null)
                {
                    var voucher = new VoucherModel
                    {
                        Name = voucherModel.Name,
                        DiscountFloat = voucherModel.DiscountFloat,
                        DiscountAmount = voucherModel.DiscountAmount,
                        Description = voucherModel.Description,
                        UpdateDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                    };

                    Debug.WriteLine(voucher.ToString());

                    _voucherRepository.Add(voucher);

                    var response = new Response
                    {
                        resultCd = 0,
                        MessageCode = "I301",
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C301",
                    };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
