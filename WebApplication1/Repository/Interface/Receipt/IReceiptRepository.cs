using WebApplication1.Data.Receipt;
using WebApplication1.Model.Receipt;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface.Receipt
{
    public interface IReceiptRepository
    {
        Response GetAll(
            string? TourishPlanId,
            ReceiptStatus? status,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 5
        );
        Response GetAllForUser(
            string? email,
            ReceiptStatus? status,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 5
        );
        Response getById(Guid id);
        Response getFullReceiptById(Guid id);
        Task<Response> Add(FullReceiptInsertModel receiptModel);
        Task<Response> AddForClient(FullReceiptClientInsertModel receiptModel);
        Task<Response> Update(FullReceiptUpdateModel receiptModel);
        Task<Response> UpdateForUser(FullReceiptUpdateModel receiptModel);
        Response Delete(Guid id);

        Response getUnpaidClient();

        Response getTopGrossTourInMonth();

        Response getTopTicketTourInMonth();

        Response getTopGrossMovingScheduleInMonth();

        Response getTopGrossStayingScheduleInMonth();

        Response getTopTicketMovingScheduleInMonth();

        Response getTopTicketStayingScheduleInMonth();
    }
}
