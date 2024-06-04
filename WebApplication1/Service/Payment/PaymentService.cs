using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TourishApi.Service.InheritanceService;
using WebApplication1.Data.Receipt;
using WebApplication1.Model;
using WebApplication1.Model.Payment;
using WebApplication1.Service.InheritanceService;

namespace TourishApi.Service.Payment
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly PayOsSetting payOsSettings;
        private readonly AppSetting _appSettings;
        private readonly ILogger<PaymentService> logger;
        private readonly ReceiptService _receiptService;

        public PaymentService(
            HttpClient httpClient,
            IOptions<PayOsSetting> _payOsSettings,
            ILogger<PaymentService> _logger,
            IOptions<AppSetting> appSettings,
            ReceiptService receiptService
        )
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            payOsSettings = _payOsSettings.Value;
            logger = _logger;
            _receiptService = receiptService;
            _appSettings = appSettings.Value;
        }

        public async Task<PaymentResponse> MakeTourPaymentAsync(PaymentRequest paymentRequest)
        {
            FullReceipt existReceipt = (FullReceipt)
                _receiptService.GetFullTourReceiptById(paymentRequest.orderCode).Data;

            PaymentRequest insertReq = paymentRequest;
            insertReq.buyerEmail = existReceipt.Email;
            insertReq.buyerPhone = existReceipt.PhoneNumber;
            insertReq.amount = (int)(
                (
                    existReceipt.OriginalPrice * existReceipt.TotalTicket
                    + existReceipt.OriginalPrice * existReceipt.TotalChildTicket
                ) * (1 - existReceipt.DiscountFloat)
                - existReceipt.DiscountAmount
            );

            insertReq.returnUrl = _appSettings.BaseUrl + "/CallPayment/pay-os/update/tour";
            insertReq.cancelUrl = _appSettings.BaseUrl + "/CallPayment/pay-os/update/tour";

            insertReq.buyerName = existReceipt.GuestName;
            insertReq.buyerAddress = "";
            insertReq.items = null;
            insertReq.description = "Roxanne tour hanh toán";

            return await MakePaymentAsync(insertReq);
        }

        public async Task<PaymentResponse> MakeServicePaymentAsync(PaymentRequest paymentRequest)
        {
            FullReceipt existReceipt = (FullReceipt)
                _receiptService.GetFullScheduleReceiptById(paymentRequest.orderCode).Data;

            PaymentRequest insertReq = paymentRequest;
            insertReq.buyerEmail = existReceipt.Email;
            insertReq.buyerPhone = existReceipt.PhoneNumber;
            insertReq.amount = (int)(
                (
                    existReceipt.OriginalPrice * existReceipt.TotalTicket
                    + existReceipt.OriginalPrice * existReceipt.TotalChildTicket
                ) * (1 - existReceipt.DiscountFloat)
                - existReceipt.DiscountAmount
            );

            insertReq.returnUrl = _appSettings.BaseUrl + "/CallPayment/pay-os/update/tour";
            insertReq.cancelUrl = _appSettings.BaseUrl + "/CallPayment/pay-os/update/tour";

            insertReq.buyerName = existReceipt.GuestName;
            insertReq.buyerAddress = "";
            insertReq.items = null;
            insertReq.description = "Roxanne tour thanh toán";

            return await MakePaymentAsync(insertReq);
        }

        public async Task<PaymentResponse> MakePaymentAsync(PaymentRequest paymentRequest)
        {
            if (paymentRequest == null)
            {
                throw new ArgumentNullException(nameof(paymentRequest));
            }
            PaymentRequest insertReq = paymentRequest;

            insertReq.expiredAt = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds();

            if (insertReq.returnUrl == null)
                insertReq.returnUrl = "https://roxanne-tour.pro/";
            if (insertReq.cancelUrl == null)
                insertReq.cancelUrl = "https://roxanne-tour.pro/";

            insertReq.signature = GenerateSignature(
                paymentRequest.amount.ToString(),
                paymentRequest.cancelUrl,
                paymentRequest.description,
                paymentRequest.orderCode,
                paymentRequest.returnUrl
            );

            logger.LogInformation(JsonSerializer.Serialize(insertReq));

            // Tạo request body từ paymentRequest
            var requestBody = new StringContent(
                JsonSerializer.Serialize(insertReq),
                Encoding.UTF8,
                "application/json"
            );

            // Thêm header x-client-idx-api-key
            _httpClient.DefaultRequestHeaders.Add("x-client-id", payOsSettings.ClientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", payOsSettings.ApiKey);

            // Gọi API POST
            var response = await _httpClient.PostAsync(
                "https://api-merchant.payos.vn/v2/payment-requests",
                requestBody
            );

            // Đảm bảo response thành công
            response.EnsureSuccessStatusCode();

            // Đọc response content
            var responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize responseContent to PaymentResponse object
            var responseData = JsonSerializer.Deserialize<PaymentResponse>(responseContent);

            return responseData;
        }

        public string GenerateSignature(
            string amount,
            string cancelUrl,
            string description,
            int orderCode,
            string returnUrl
        )
        {
            // Construct the data string in the specified format and sort alphabetically
            var dataString =
                $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";
            //var sortedDataString = SortStringAlphabetically(dataString);

            // Convert checksum key and data to bytes
            var keyBytes = Encoding.UTF8.GetBytes(payOsSettings.ChecksumKey);
            var dataBytes = Encoding.UTF8.GetBytes(dataString);

            // Compute the HMAC_SHA256 hash
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(dataBytes);
                var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hashString;
            }
        }
    }
}
