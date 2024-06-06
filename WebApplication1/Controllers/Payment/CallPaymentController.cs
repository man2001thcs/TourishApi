using System.Threading.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TourishApi.Service.InheritanceService;
using TourishApi.Service.Payment;
using WebApplication1.Data.Receipt;
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

        public CallPaymentController(
            PaymentService paymentService,
            ReceiptService receiptService,
            IOptionsMonitor<AppSetting> optionsMonitor
        )
        {
            _paymentService = paymentService;
            _receiptService = receiptService;
            _appSettings = optionsMonitor.CurrentValue;
        }

        // [HttpPost("request")]
        // public async Task<IActionResult> CreateNew(PaymentRequest request)
        // {
        //     return Ok(await _paymentService.MakePaymentAsync(request));
        // }

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

        [HttpPost("tour/request/cancel")]
        public async Task<IActionResult> CancelTourReq(PaymentCancelRequest request)
        {
            return Ok(
                await _paymentService.CancelTourPaymentAsync(request.id, request.cancellationReason)
            );
        }

        [HttpPost("service/request/cancel")]
        public async Task<IActionResult> CancelServiceReq(PaymentCancelRequest request)
        {
            return Ok(
                await _paymentService.CancelServicePaymentAsync(
                    request.id,
                    request.cancellationReason
                )
            );
        }

        [HttpGet("tour/request")]
        public async Task<IActionResult> GetTourReq(string id)
        {
            return Ok(await _paymentService.GetTourPaymentRequest(id));
        }

        [HttpGet("service/request")]
        public async Task<IActionResult> GetServiceReq(string id)
        {
            return Ok(await _paymentService.GetServicePaymentRequest(id));
        }

        [Authorize]
        [HttpGet("tour/check-request")]
        public async Task<IActionResult> RedirectTourReq(string id)
        {
            var paymentId = await _paymentService.CheckTourRequest(id);

            if (paymentId.Length > 0)
                return Ok(new Response { resultCd = 0, MessageCode = "I510" });
            else
                return Ok(new Response { resultCd = 0, MessageCode = "I515-m" });
        }

        [Authorize]
        [HttpGet("service/check-request")]
        public async Task<IActionResult> RedirectServiceReq(string id)
        {
            var paymentId = await _paymentService.CheckServiceRequest(id);
            if (paymentId.Length > 0)
                return Ok(new Response { resultCd = 0, MessageCode = "I510" });
            else
                return Ok(new Response { resultCd = 0, MessageCode = "I515-m" });
        }

        [HttpGet("pay-os/update/tour")]
        public async Task<IActionResult> UpdateReceipt(string id, string orderCode, string status)
        {
            await _receiptService.thirdPartyPaymentFullReceiptStatusChange(id, orderCode, status);

            return Redirect(_appSettings.ClientUrl + "/user/receipt/list");
        }

        [HttpGet("pay-os/update/service")]
        public async Task<IActionResult> UpdateServiceReceipt(
            string id,
            string orderCode,
            string status
        )
        {
            await _receiptService.thirdPartyPaymentFullServiceReceiptStatusChange(
                id,
                orderCode,
                status
            );

            var receipt = (FullScheduleReceipt)
                _receiptService.GetFullScheduleReceiptById(int.Parse(orderCode)).Data;

            if (receipt == null)
            {
                if (receipt.ServiceSchedule.MovingScheduleId != null)
                    return Redirect(_appSettings.ClientUrl + "/user/moving/receipt/list");
                else
                    return Redirect(_appSettings.ClientUrl + "/user/staying/receipt/list");
            }

            return Redirect(_appSettings.ClientUrl + "/user/moving/receipt/list");
        }

        [HttpPost("pay-os/web-hook/tour")]
        public async Task<IActionResult> WebHookTour(PaymentWebHookRequest paymentWebHookRequest)
        {
            var isSignatureTrue = _paymentService.isTourWebHookReqValid(
                paymentWebHookRequest.data,
                paymentWebHookRequest.signature
            );

            // if (isSignatureTrue && paymentWebHookRequest.data.code.Equals("00"))
            //     await _receiptService.thirdPartyPaymentFullReceiptStatusChange(
            //         paymentWebHookRequest.data.paymentLinkId,
            //         paymentWebHookRequest.data.orderCode.ToString(),
            //         "PAID"
            //     );

            return Ok(paymentWebHookRequest);
        }

        [HttpPost("pay-os/web-hook/service")]
        public async Task<IActionResult> WebHookService(PaymentWebHookRequest paymentWebHookRequest)
        {
            var isSignatureTrue = _paymentService.isServiceWebHookReqValid(
                paymentWebHookRequest.data,
                paymentWebHookRequest.signature
            );

            // if (isSignatureTrue && paymentWebHookRequest.data.code.Equals("00"))
            //     await _receiptService.thirdPartyPaymentFullServiceReceiptStatusChange(
            //         paymentWebHookRequest.data.paymentLinkId,
            //         paymentWebHookRequest.data.orderCode.ToString(),
            //         "PAID"
            //     );

            return Ok(paymentWebHookRequest);
        }
    }
}
