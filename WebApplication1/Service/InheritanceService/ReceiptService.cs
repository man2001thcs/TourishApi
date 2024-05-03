using WebApplication1.Data.Receipt;
using WebApplication1.Model.Receipt;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface.Receipt;

namespace TourishApi.Service.InheritanceService
{
    public class ReceiptService
    {
        private readonly IReceiptRepository _receiptRepository;

        private readonly char[] delimiter = new char[] { ';' };

        public ReceiptService(IReceiptRepository receiptRepository
            )
        {
            _receiptRepository = receiptRepository;
        }

        public async Task<Response> CreateNew(FullReceiptInsertModel receiptInsertModel)
        {
            try
            {

                var receiptReturn = await _receiptRepository.Add(receiptInsertModel);

                var receiptReturnId = (Guid)receiptReturn.returnId;


                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I511",
                };
                return response;


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

        public async Task<Response> CreateNewForClient(FullReceiptClientInsertModel receiptInsertModel)
        {
            try
            {

                var receiptReturn = await _receiptRepository.AddForClient(receiptInsertModel);

                var receiptReturnId = (Guid)receiptReturn.returnId;


                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I511",
                    Data = receiptReturn.Data
                };
                return response;


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

        public Response DeleteById(Guid id)
        {

            try
            {
                _receiptRepository.Delete(id);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I513",
                };
                return response;
            }
            catch
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C514",
                };
                return response;
            }
        }


        public Response GetFullReceiptById(Guid id)
        {
            try
            {
                var receipt = _receiptRepository.getFullReceiptById(id);
                if (receipt.Data == null)
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C510",
                    };
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

        public Response GetAll(string? tourishPlanId, string? sortBy, string? sortDirection, int page = 1, int pageSize = 5, ReceiptStatus status = ReceiptStatus.Created)
        {
            try
            {
                var receiptList = _receiptRepository.GetAll(tourishPlanId, status, sortBy, sortDirection, page, pageSize);
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

        public Response GetAllForUser(string? email, string? sortBy, string? sortDirection, int page = 1, int pageSize = 5, ReceiptStatus status = ReceiptStatus.Created)
        {
            try
            {
                var receiptList = _receiptRepository.GetAllForUser(email, status, sortBy, sortDirection, page, pageSize);
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

        public Response GetById(Guid id)
        {
            try
            {
                var receipt = _receiptRepository.getById(id);
                if (receipt.Data == null)
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C510",
                    };
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

        public async Task<Response> UpdateReceiptById(Guid fullReceiptId, FullReceiptUpdateModel receiptModel)
        {
            try
            {
                await _receiptRepository.Update(receiptModel);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I512",
                };
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

        public async Task<Response> UpdateReceiptForUserById(Guid fullReceiptId, FullReceiptUpdateModel receiptModel)
        {
            try
            {
                await _receiptRepository.UpdateForUser(receiptModel);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I512",
                };
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
    }
}
