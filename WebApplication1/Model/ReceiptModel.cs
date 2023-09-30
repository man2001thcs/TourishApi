using WebApplication1.Data;

namespace WebApplication1.Model
{
    public class ReceiptModel
    {
        public Guid ReceiptId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public ReceiptStatus Status { get; set; }

        // Relationship
        public ICollection<FullReceipt> FullReceiptList { get; set; }
    }

    public class ReceiptInsertModel
    {
        public Guid UserId { get; set; }
        public string? Description { get; set; }
        public Guid? VoucherId { get; set; }
        public string TransportMethod { get; set; }
        public ReceiptStatus Status { get; set; }
        public string SingleReceiptString { get; set; }
    }

    public class ReceiptUpdateModel
    {
        public Guid ReceiptId { get; set; }
        public Guid? VoucherId { get; set; }
        public required string Description { get; set; }
        public string? TransportMethod { get; set; }
        public DateTime? CompleteDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public ReceiptStatus Status { get; set; }

    }

    public class FullReceiptModel
    {
        public Guid ProductId { get; set; }
        public int TotalNumber { get; set; }
        public double SinglePrice { get; set; }
        public float DiscountFloat { get; set; }
        public double DiscountAmount { get; set; }

    }
}
