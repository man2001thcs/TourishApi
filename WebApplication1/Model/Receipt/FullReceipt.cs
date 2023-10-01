using WebApplication1.Data.Receipt;

namespace WebApplication1.Model.Receipt
{
    public class FullReceiptModel
    {
        public Guid FullReceiptId { get; set; }
        public Guid ReceiptId { get; set; }
        public Guid ServiceId { get; set; }
        public ServiceType ServiceType { get; set; }
        public int TotalNumber { get; set; }
        public double SinglePrice { get; set; }
        public float DiscountFloat { get; set; }
        public double DiscountAmount { get; set; }

    }
}
