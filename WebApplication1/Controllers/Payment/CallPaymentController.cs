using System.Threading.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TourishApi.Service.InheritanceService;
using TourishApi.Service.Payment;
using WebApplication1.Model;
using WebApplication1.Model.Payment;
using WebApplication1.Model.VirtualModel;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallPaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly ReceiptService _receiptService;
        private readonly AppSetting _appSettings;

        private readonly char[] delimiter = new char[] { ';' };

        public CallPaymentController(PaymentService paymentService, ReceiptService receiptService, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _paymentService = paymentService;
            _receiptService = receiptService;
            _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpPost("request")]
        public async Task<IActionResult> CreateNew(PaymentRequest request)
        {
            return Ok(await _paymentService.MakePaymentAsync(request));
        }

        [HttpPost("tour/request")]
        public async Task<IActionResult> CreateNewTourReq(PaymentRequest request)
        {
            return Ok(await _paymentService.MakeTourPaymentAsync(request));
        }

        [HttpPost("service/request")]
        public async Task<IActionResult> CreateNewServiceReq(PaymentRequest request)
        {
            return Ok(await _paymentService.MakeServicePaymentAsync(request));
        }

        [Authorize]
        [HttpGet("pay-os/update/tour")]
        public async Task<IActionResult> UpdateReceipt(
            string id,
            string code,
            string orderCode,
            bool cancel,
            string status
        )
        {
            var request = new PaymentChangeStatusReq();
            request.id = id;
            request.code = code;
            request.orderCode = orderCode;
            request.status = status;
            request.cancel = cancel;

            _receiptService.thirdPartyPaymentFullReceiptStatusChange(request);

            return Redirect(_appSettings.ClientUrl + "/user/receipt/list");
        }

        [Authorize]
        [HttpGet("pay-os/update/service")]
        public async Task<IActionResult> UpdateServiceReceipt(
            string id,
            string code,
            string orderCode,
            bool cancel,
            string status
        )
        {
            var request = new PaymentChangeStatusReq();
            request.id = id;
            request.code = code;
            request.orderCode = orderCode;
            request.status = status;
            request.cancel = cancel;

            _receiptService.thirdPartyPaymentFullServiceReceiptStatusChange(request);

            return Redirect(_appSettings.ClientUrl + "/user/receipt/list");
        }
    }
}
