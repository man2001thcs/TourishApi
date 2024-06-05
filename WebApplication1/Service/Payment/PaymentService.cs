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

            insertReq.returnUrl = _appSettings.BaseUrl + "/api/CallPayment/pay-os/update/tour";
            insertReq.cancelUrl = _appSettings.BaseUrl + "/api/CallPayment/pay-os/update/tour";

            insertReq.buyerName = existReceipt.GuestName;
            insertReq.buyerAddress = "";
            insertReq.items = null;
            insertReq.description = "Roxanne tour thanh toán";

            var response = await MakePaymentAsync(
                insertReq,
                payOsSettings.ClientId,
                payOsSettings.ApiKey,
                payOsSettings.ChecksumKey
            );

            logger.LogInformation(JsonSerializer.Serialize(response));

            if (response.data != null)
                _receiptService.thirdPartyPaymentFullReceiptStatusChange(
                    response.data.paymentLinkId,
                    response.data.orderCode.ToString(),
                    response.data.status
                );

            return response;
        }

        public async Task<PaymentResponse> CancelTourPaymentAsync(string id, string reason)
        {
            var response = await CancelPaymentAsync(
                id,
                reason,
                payOsSettings.ClientId,
                payOsSettings.ApiKey
            );
            if (response.data != null)
                _receiptService.thirdPartyPaymentFullReceiptStatusChange(
                    response.data.paymentLinkId,
                    response.data.orderCode.ToString(),
                    response.data.status
                );

            return response;
        }

        public async Task<PaymentGetResponse> GetTourPaymentRequest(string id)
        {
            var response = await GetPaymentRequest(
                id,
                payOsSettings.ClientId,
                payOsSettings.ApiKey
            );

            logger.LogInformation(JsonSerializer.Serialize(response));

            if (response.data != null)
                _receiptService.thirdPartyPaymentFullReceiptStatusChange(
                    response.data.id,
                    response.data.orderCode.ToString(),
                    response.data.status
                );
            return response;
        }

        public async Task<PaymentResponse> CancelServicePaymentAsync(string id, string reason)
        {
            var response = await CancelPaymentAsync(
                id,
                reason,
                payOsSettings.ServiceClientId,
                payOsSettings.ServiceApiKey
            );
            if (response.data != null)
                logger.LogInformation(JsonSerializer.Serialize(response));
            if (response.data != null)
                _receiptService.thirdPartyPaymentFullServiceReceiptStatusChange(
                    response.data.paymentLinkId,
                    response.data.orderCode.ToString(),
                    response.data.status
                );
            return response;
        }

        public async Task<PaymentGetResponse> GetServicePaymentRequest(string id)
        {
            var response = await GetPaymentRequest(
                id,
                payOsSettings.ServiceClientId,
                payOsSettings.ServiceApiKey
            );
            if (response.data != null)
                logger.LogInformation(JsonSerializer.Serialize(response));
            if (response.data != null)
                _receiptService.thirdPartyPaymentFullServiceReceiptStatusChange(
                    response.data.id,
                    response.data.orderCode.ToString(),
                    response.data.status
                );
            return response;
        }

        public async Task<PaymentResponse> MakeServicePaymentAsync(PaymentRequest paymentRequest)
        {
            FullScheduleReceipt existReceipt = (FullScheduleReceipt)
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

            insertReq.returnUrl = _appSettings.BaseUrl + "/api/CallPayment/pay-os/update/service";
            insertReq.cancelUrl = _appSettings.BaseUrl + "/api/CallPayment/pay-os/update/service";

            insertReq.buyerName = existReceipt.GuestName;
            insertReq.buyerAddress = "";
            insertReq.items = null;
            insertReq.description = "Roxanne tour thanh toán";

            var response = await MakePaymentAsync(
                insertReq,
                payOsSettings.ServiceClientId,
                payOsSettings.ServiceApiKey,
                payOsSettings.ServiceChecksumKey
            );
            if (response.data != null)
                _receiptService.thirdPartyPaymentFullServiceReceiptStatusChange(
                    response.data.paymentLinkId,
                    response.data.orderCode.ToString(),
                    response.data.status
                );

            return response;
        }

        public async Task<PaymentResponse> MakePaymentAsync(
            PaymentRequest paymentRequest,
            string clientId,
            string apiKey,
            string checkSumKey
        )
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
                paymentRequest.returnUrl,
                checkSumKey
            );

            logger.LogInformation(JsonSerializer.Serialize(insertReq));

            // Tạo request body từ paymentRequest
            var requestBody = new StringContent(
                JsonSerializer.Serialize(insertReq),
                Encoding.UTF8,
                "application/json"
            );

            // Thêm header x-client-idx-api-key
            _httpClient.DefaultRequestHeaders.Add("x-client-id", clientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);

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

        public async Task<PaymentResponse> CancelPaymentAsync(
            string id,
            string reason,
            string clientId,
            string apiKey
        )
        {
            var insertReq = new PaymentCancelRequest();
            insertReq.cancellationReason = reason;

            // Tạo request body từ paymentRequest
            var requestBody = new StringContent(
                JsonSerializer.Serialize(insertReq),
                Encoding.UTF8,
                "application/json"
            );

            // Thêm header x-client-idx-api-key
            _httpClient.DefaultRequestHeaders.Add("x-client-id", clientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);

            // Gọi API POST
            var response = await _httpClient.PostAsync(
                $"https://api-merchant.payos.vn/v2/payment-requests/{id}/cancel",
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

        public async Task<PaymentGetResponse> GetPaymentRequest(
            string id,
            string clientId,
            string apiKey
        )
        {
            // Thêm header x-client-idx-api-key
            _httpClient.DefaultRequestHeaders.Add("x-client-id", clientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);

            var response = await _httpClient.GetAsync(
                $"https://api-merchant.payos.vn/v2/payment-requests/{id}"
            );

            var responseContent = await response.Content.ReadAsStringAsync();

            logger.LogInformation(responseContent);

            var responseData = JsonSerializer.Deserialize<PaymentGetResponse>(responseContent);

            return responseData;
        }

        public string GenerateSignature(
            string amount,
            string cancelUrl,
            string description,
            int orderCode,
            string returnUrl,
            string checkSumKey
        )
        {
            // Construct the data string in the specified format and sort alphabetically
            var dataString =
                $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";
            //var sortedDataString = SortStringAlphabetically(dataString);

            // Convert checksum key and data to bytes
            var keyBytes = Encoding.UTF8.GetBytes(checkSumKey);
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
