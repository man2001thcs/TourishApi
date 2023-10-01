using WebApplication1.Data.Receipt;

namespace WebApplication1.Model.Receipt
{
    public class TotalReceiptModel
    {
        public Guid ReceiptId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public string Description { get; set; }
        public ReceiptStatus Status { get; set; }

        public string SingleReceiptString { get; set; }
    }

    public class TotalReceiptInsertModel
    {
        public Guid ReceiptId { get; set; }
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public ReceiptStatus Status { get; set; }

        public string SingleReceiptString { get; set; }
    }
}
