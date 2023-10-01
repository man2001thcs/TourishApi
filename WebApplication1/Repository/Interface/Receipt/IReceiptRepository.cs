using WebApplication1.Data.Receipt;
using WebApplication1.Model.Receipt;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface.Receipt
{
    public interface IReceiptRepository
    {
        Response GetAll(string? userId, ReceiptStatus? status, string? sortBy, int page = 1, int pageSize = 5);
        Response getById(Guid id);
        Task<Response> Add(TotalReceiptInsertModel receiptModel);
        Task<Response> Update(TotalReceiptModel receiptModel);
        Response Delete(Guid id);
    }
}
