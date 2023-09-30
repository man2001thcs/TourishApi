using WebApplication1.Data;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface
{
    public interface IReceiptRepository
    {
        Response GetAll(string? userId, ReceiptStatus? status, string? sortBy, int page = 1, int pageSize = 5);
        Response getById(Guid id);
        Task<Response> Add(ReceiptInsertModel receiptModel);
        Task<Response> Update(ReceiptUpdateModel receiptModel);
        Response Delete(Guid id);
    }
}
