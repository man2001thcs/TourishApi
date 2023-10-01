using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data.Receipt
{
    public enum ServiceType
    {
        Transport = 0, Staying = 1, Eating = 2
    }

    [Table("FullReceipt")]
    public class FullReceipt
    {
        public Guid FullReceiptId { get; set; }
        public Guid ReceiptId { get; set; }
        public ServiceType ServiceType { get; set; }
        public Guid ServiceId { get; set; }
        public int TotalNumber { get; set; }
        public double SinglePrice { get; set; }
        public float DiscountFloat { get; set; }
        public double DiscountAmount { get; set; }

        // Relationship
        public TotalReceipt TotalReceipt { get; set; }

    }
}
