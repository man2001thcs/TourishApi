using MailKit;
using Org.BouncyCastle.Utilities;
using TourishApi.Service.InheritanceService.Schedule;
using TourishApi.Service.Payment;
using WebApplication1.Data;
using WebApplication1.Data.Receipt;
using WebApplication1.Data.Schedule;
using WebApplication1.Model;
using WebApplication1.Model.Payment;
using WebApplication1.Model.Receipt;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.InheritanceRepo.Receipt;
using WebApplication1.Service;

namespace TourishApi.Service.InheritanceService
{
    public class ReceiptService
    {
        private readonly ReceiptRepository _receiptRepository;
        private readonly ISendMailService _sendMailService;
        private readonly TourishPlanService _tourishPlanService;
        private readonly ILogger<ReceiptService> logger;
        private readonly MovingScheduleService _movingScheduleService;
        private readonly StayingScheduleService _stayingScheduleService;

        private readonly char[] delimiter = new char[] { ';' };

        public ReceiptService(ReceiptRepository receiptRepository, ISendMailService sendMailService,
        MovingScheduleService movingScheduleService,
        StayingScheduleService stayingScheduleService,
        ILogger<ReceiptService> _logger,
        TourishPlanService tourishPlanService)
        {
            _receiptRepository = receiptRepository;
            _sendMailService = sendMailService;
            _tourishPlanService = tourishPlanService;
            _movingScheduleService = movingScheduleService;
            _stayingScheduleService = stayingScheduleService;
            logger = _logger;
        }

        public async Task<Response> CreateNew(FullReceiptInsertModel receiptInsertModel)
        {
            try
            {
                if (receiptInsertModel.TourishPlanId != null)
                {
                    var receiptReturn = await _receiptRepository.AddTourReceipt(receiptInsertModel);

                    var response = new Response { resultCd = 0, MessageCode = "I511", };
                    return response;
                }
                else
                {
                    var receiptReturn = await _receiptRepository.AddServiceReceipt(
                        receiptInsertModel
                    );


                    var response = new Response { resultCd = 0, MessageCode = "I511", };
                    return response;
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C515",
                    Error = ex.Message,
                    Data = ex
                };
                return response;
            }
        }

        public async Task<Response> CreateNewForClient(
            FullReceiptClientInsertModel receiptInsertModel
        )
        {
            try
            {
                if (receiptInsertModel.TourishPlanId != null)
                {
                    var receiptReturn = await _receiptRepository.AddTourReceiptForClient(
                        receiptInsertModel
                    );

                    return receiptReturn;
                }
                else
                {
                    var receiptReturn = await _receiptRepository.AddScheduleReceiptForClient(
                        receiptInsertModel
                    );

                    return receiptReturn;
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C515",
                    Error = ex.Message,
                    Data = ex
                };
                return response;
            }
        }

        public Response DeleteTourReceiptById(int id)
        {
            try
            {
                _receiptRepository.DeleteTourReceipt(id);
                var response = new Response { resultCd = 0, MessageCode = "I513", };
                return response;
            }
            catch
            {
                var response = new Response { resultCd = 1, MessageCode = "C514", };
                return response;
            }
        }

        public Response DeleteScheduleReceiptById(int id)
        {
            try
            {
                _receiptRepository.DeleteScheduleReceipt(id);
                var response = new Response { resultCd = 0, MessageCode = "I513", };
                return response;
            }
            catch
            {
                var response = new Response { resultCd = 1, MessageCode = "C514", };
                return response;
            }
        }

        public Response GetFullTourReceiptById(int id)
        {
            try
            {
                var receipt = _receiptRepository.getFullTourReceiptById(id);
                if (receipt.Data == null)
                {
                    var response = new Response { resultCd = 1, MessageCode = "C510", };
                    return response;
                }
                else
                {
                    return receipt;
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response GetFullScheduleReceiptById(int id)
        {
            try
            {
                var receipt = _receiptRepository.getFullScheduleReceiptById(id);
                if (receipt.Data == null)
                {
                    var response = new Response { resultCd = 1, MessageCode = "C510", };
                    return response;
                }
                else
                {
                    return receipt;
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response GetAllTourReceipt(
            string? tourishPlanId,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 5,
            FullReceiptStatus status = FullReceiptStatus.Created
        )
        {
            try
            {
                var receiptList = _receiptRepository.GetAllTourReceipt(
                    tourishPlanId,
                    status,
                    sortBy,
                    sortDirection,
                    page,
                    pageSize
                );
                return receiptList;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response GetAllScheduleReceipt(
            string? movingScheduleId,
            string? stayingScheduleId,
            ScheduleType? scheduleType,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 5,
            FullReceiptStatus status = FullReceiptStatus.Created
        )
        {
            try
            {
                var receiptList = _receiptRepository.GetAllScheduleReceipt(
                    movingScheduleId,
                    stayingScheduleId,
                    scheduleType,
                    status,
                    sortBy,
                    sortDirection,
                    page,
                    pageSize
                );
                return receiptList;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response GetAllTourReceiptForUser(
            string? email,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 5,
            FullReceiptStatus status = FullReceiptStatus.Created
        )
        {
            try
            {
                var receiptList = _receiptRepository.GetAllTourReceiptForUser(
                    email,
                    status,
                    sortBy,
                    sortDirection,
                    page,
                    pageSize
                );
                return receiptList;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response GetAllScheduleReceiptForUser(
            string? email,
            ScheduleType? scheduleType,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 5,
            FullReceiptStatus status = FullReceiptStatus.Created
        )
        {
            try
            {
                var receiptList = _receiptRepository.GetAllScheduleReceiptForUser(
                    email,
                    scheduleType,
                    status,
                    sortBy,
                    sortDirection,
                    page,
                    pageSize
                );
                return receiptList;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response GetTotalTourReceiptById(Guid id)
        {
            try
            {
                var receipt = _receiptRepository.getTotalTourReceiptById(id);
                if (receipt.Data == null)
                {
                    var response = new Response { resultCd = 1, MessageCode = "C510", };
                    return response;
                }
                else
                {
                    return receipt;
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response GetTotalScheduleReceiptById(Guid id)
        {
            try
            {
                var receipt = _receiptRepository.getTotalScheduleReceiptById(id);
                if (receipt.Data == null)
                {
                    var response = new Response { resultCd = 1, MessageCode = "C510", };
                    return response;
                }
                else
                {
                    return receipt;
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public async Task<Response> UpdateReceiptById(
            Guid fullReceiptId,
            FullReceiptUpdateModel receiptModel
        )
        {
            try
            {
                if (receiptModel.TourishPlanId != null)
                    await _receiptRepository.UpdateTourReceipt(receiptModel);
                else
                    await _receiptRepository.UpdateScheduleReceipt(receiptModel);

                var response = new Response { resultCd = 0, MessageCode = "I512", };
                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public async Task<Response> UpdateReceiptForUserById(
            Guid fullReceiptId,
            FullReceiptUpdateModel receiptModel
        )
        {
            try
            {
                if (receiptModel.TourishPlanId != null)
                    await _receiptRepository.UpdateTourReceiptForUser(receiptModel);
                else
                    await _receiptRepository.UpdateScheduleReceiptForUser(receiptModel);

                var response = new Response { resultCd = 0, MessageCode = "I512", };
                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response getUnpaidTourClient()
        {
            try
            {
                return _receiptRepository.getUnpaidTourClient();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response getUnpaidMovingScheduleClient()
        {
            try
            {
                return _receiptRepository.getUnpaidMovingScheduleClient();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response getUnpaidStayingScheduleClient()
        {
            try
            {
                return _receiptRepository.getUnpaidStayingScheduleClient();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response getTopGrossTourInMonth()
        {
            try
            {
                return _receiptRepository.getTopGrossTourInMonth();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response getTopTicketTourInMonth()
        {
            try
            {
                return _receiptRepository.getTopTicketTourInMonth();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response getTopGrossMovingScheduleInMonth()
        {
            try
            {
                return _receiptRepository.getTopGrossMovingScheduleInMonth();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response getTopGrossStayingScheduleInMonth()
        {
            try
            {
                return _receiptRepository.getTopGrossStayingScheduleInMonth();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response getTopTicketMovingScheduleInMonth()
        {
            try
            {
                return _receiptRepository.getTopTicketMovingScheduleInMonth();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response getTopTicketStayingScheduleInMonth()
        {
            try
            {
                return _receiptRepository.getTopTicketStayingScheduleInMonth();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response getGrossStayingScheduleInYear()
        {
            try
            {
                return _receiptRepository.getGrossStayingScheduleInYear();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response getGrossMovingScheduleInYear()
        {
            try
            {
                return _receiptRepository.getGrossMovingScheduleInYear();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response getGrossTourishPlanInYear()
        {
            try
            {
                return _receiptRepository.getGrossTourishPlanInYear();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public async Task<Response> thirdPartyPaymentFullServiceReceiptStatusChange(
            string paymentId,
        string orderId, string status
        )
        {
            try
            {
                var existReceipt = (FullScheduleReceipt)_receiptRepository.getFullScheduleReceiptById(int.Parse(orderId)).Data;
                if (existReceipt != null)
                {
                    if ((int)existReceipt.Status < 2)
                    {
                        var response = await _receiptRepository.thirdPartyPaymentFullServiceReceiptStatusChange(
                                                                paymentId, orderId, status
                                                            );

                        if (status.Equals("PAID"))
                            await SendServiceReceiptToEmail(orderId);

                        return response;
                    }

                }
                return new Response();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public async Task<Response> thirdPartyPaymentFullReceiptStatusChange(
            string paymentId,
        string orderId, string status
        )
        {
            try
            {
                var existReceipt = (FullReceipt)_receiptRepository.getFullTourReceiptById(int.Parse(orderId)).Data;
                if (existReceipt != null)
                {
                    if ((int)existReceipt.Status < 2)
                    {
                        var response = await _receiptRepository.thirdPartyPaymentFullReceiptStatusChange(
                                                                paymentId, orderId, status
                                                            );

                        if (status.Equals("PAID"))
                            await SendTourReceiptToEmail(orderId);

                        return response;
                    }

                }
                return new Response();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                    Error = ex.Message
                };
                return response;
            }
        }

        public async Task<Response> SendTourReceiptToEmail(string orderId)
        {
            try
            {
                var existReceipt = (FullReceipt)_receiptRepository.getFullTourReceiptById(int.Parse(orderId)).Data;

                if (existReceipt != null)
                {
                    var totalPrice = (existReceipt.OriginalPrice * existReceipt.TotalTicket
                        + existReceipt.OriginalPrice * existReceipt.TotalChildTicket) * (1 - existReceipt.DiscountFloat)
                    - existReceipt.DiscountAmount;

                    var tourishPlan = (TourishPlan)_tourishPlanService.GetById(existReceipt.TourishSchedule.TourishPlanId).Data;

                    var mailContent = new MailContent
                    {
                        To = existReceipt.Email,
                        Subject = "Roxanne: Thanh toán thành công: " + tourishPlan.TourName,
                        Body =
                        "<html>"
                        + "<head>"
                        + "<style>"
                        + "body { font-family: Arial, sans-serif; background-color: #f4f4f4; }"
                        + ".container { max-width: 600px; margin: 0 auto; padding: 20px; background-color: #fff; border-radius: 10px; box-shadow: 0px 0px 10px 0px rgba(0,0,0,0.1); }"
                        + ".message { margin-bottom: 20px; }"
                        + ".message p { margin: 0; font-size: 16px; margin-bottom: 10px; }"
                        + ".btn { display: inline-block; background-color: #007bff; color: #fff !important; text-decoration: none; padding: 10px 20px; border-radius: 5px; }"
                        + "table { width: 100%; border-collapse: collapse; margin-top: 20px; }"
                        + "table, th, td { border: 1px solid #ddd; }"
                        + "th, td { padding: 12px; text-align: left; }"
                        + "th { background-color: #f2f2f2; }"
                        + "</style>"
                        + "</head>"
                        + "<body>"
                        + "<div class='container'>"
                        + "<div class='message'>"
                        + "<p style='font-size: 18px; margin-bottom: 10px;'>Xin chào <strong>"
                        + existReceipt.GuestName
                        + "</strong>.</p>"
                        + "<p style='font-size: 18px; margin-bottom: 10px;'>Xin cảm ơn bạn đã thanh toán dịch vụ của chúng tôi. Sau đây là chi tiết thông tin hóa đơn: </p>"
                        + "<table>"
                        + "<tr>"
                        + "<th>Số vé người lớn</th>"
                        + "<td>" + existReceipt.TotalTicket + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<th>Số vé trẻ em</th>"
                        + "<td>" + existReceipt.TotalChildTicket + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<th>Tổng giá</th>"
                        + "<td>" + totalPrice + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<th>Tên tour</th>"
                        + "<td>" + tourishPlan.TourName + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<th>Giá đơn vé</th>"
                        + "<td>" + existReceipt.OriginalPrice + "</td>"
                        + "</tr>"
                        + "</table>"
                        + "</div>"
                        + "</div>"
                        + "</body>"
                        + "</html>"
                    };

                    return await _sendMailService.SendMail(mailContent);
                }

                return new Response();
            }
            catch (Exception ex)
            {
                return new Response();
            }
        }

        public async Task<Response> SendServiceReceiptToEmail(string orderId)
        {
            try
            {
                var existReceipt = (FullScheduleReceipt)_receiptRepository.getFullScheduleReceiptById(int.Parse(orderId)).Data;

                if (existReceipt != null)
                {
                    var totalPrice = (existReceipt.OriginalPrice * existReceipt.TotalTicket
                        + existReceipt.OriginalPrice * existReceipt.TotalChildTicket) * (1 - existReceipt.DiscountFloat)
                    - existReceipt.DiscountAmount;

                    var serviceName = "";
                    if (existReceipt.ServiceSchedule.MovingScheduleId != null)
                    {
                        var schedule = (MovingSchedule)_movingScheduleService.GetById(existReceipt.ServiceSchedule.MovingScheduleId ?? new Guid()).Data;

                        if (schedule != null)
                            serviceName = schedule.Name;
                    }

                    else if (existReceipt.ServiceSchedule.StayingScheduleId != null)
                    {
                        var schedule = (StayingSchedule)_stayingScheduleService.GetById(existReceipt.ServiceSchedule.StayingScheduleId ?? new Guid()).Data;

                        if (schedule != null)
                            serviceName = schedule.Name;
                    }

                    var mailContent = new MailContent
                    {
                        To = existReceipt.Email,
                        Subject = "Roxanne: Thanh toán thành công: " + serviceName,
                        Body =
                        "<html>"
                        + "<head>"
                        + "<style>"
                        + "body { font-family: Arial, sans-serif; background-color: #f4f4f4; }"
                        + ".container { max-width: 600px; margin: 0 auto; padding: 20px; background-color: #fff; border-radius: 10px; box-shadow: 0px 0px 10px 0px rgba(0,0,0,0.1); }"
                        + ".message { margin-bottom: 20px; }"
                        + ".message p { margin: 0; font-size: 16px; margin-bottom: 10px; }"
                        + ".btn { display: inline-block; background-color: #007bff; color: #fff !important; text-decoration: none; padding: 10px 20px; border-radius: 5px; }"
                        + "table { width: 100%; border-collapse: collapse; margin-top: 20px; }"
                        + "table, th, td { border: 1px solid #ddd; }"
                        + "th, td { padding: 12px; text-align: left; }"
                        + "th { background-color: #f2f2f2; }"
                        + "</style>"
                        + "</head>"
                        + "<body>"
                        + "<div class='container'>"
                        + "<div class='message'>"
                        + "<p style='font-size: 18px; margin-bottom: 10px;'>Xin chào <strong>"
                        + existReceipt.GuestName
                        + "</strong>.</p>"
                        + "<p style='font-size: 18px; margin-bottom: 10px;'>Xin cảm ơn bạn đã thanh toán dịch vụ của chúng tôi. Sau đây là chi tiết thông tin hóa đơn: </p>"
                        + "<table>"
                        + "<tr>"
                        + "<th>Số vé người lớn</th>"
                        + "<td>" + existReceipt.TotalTicket + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<th>Số vé trẻ em</th>"
                        + "<td>" + existReceipt.TotalChildTicket + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<th>Tổng giá</th>"
                        + "<td>" + totalPrice + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<th>Tên dịch vụ</th>"
                        + "<td>" + serviceName + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<th>Giá đơn vé</th>"
                        + "<td>" + existReceipt.OriginalPrice + "</td>"
                        + "</tr>"
                        + "</table>"
                        + "</div>"
                        + "</div>"
                        + "</body>"
                        + "</html>"
                    };

                    return await _sendMailService.SendMail(mailContent);
                }

                return new Response();
            }
            catch (Exception ex)
            {
                return new Response();
            }
        }
    }
}
