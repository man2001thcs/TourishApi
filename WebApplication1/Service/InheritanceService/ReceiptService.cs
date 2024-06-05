using TourishApi.Service.Payment;
using WebApplication1.Data.Receipt;
using WebApplication1.Model.Payment;
using WebApplication1.Model.Receipt;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.InheritanceRepo.Receipt;

namespace TourishApi.Service.InheritanceService
{
    public class ReceiptService
    {
        private readonly ReceiptRepository _receiptRepository;

        private readonly char[] delimiter = new char[] { ';' };

        public ReceiptService(ReceiptRepository receiptRepository)
        {
            _receiptRepository = receiptRepository;
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

        public Response thirdPartyPaymentFullServiceReceiptStatusChange(
            string paymentId,
        string orderId, string status
        )
        {
            try
            {
                return _receiptRepository.thirdPartyPaymentFullServiceReceiptStatusChange(
                    paymentId, orderId, status
                );
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

        public Response thirdPartyPaymentFullReceiptStatusChange(
            string paymentId,
        string orderId, string status
        )
        {
            try
            {
                return _receiptRepository.thirdPartyPaymentFullReceiptStatusChange(
                    paymentId, orderId, status
                );
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
    }
}
