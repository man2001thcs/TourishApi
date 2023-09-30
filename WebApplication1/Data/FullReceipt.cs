using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    [Table("FullReceipt")]
    public class FullReceipt
    {
        public Guid ReceiptId { get; set; }
        public Guid ProductId { get; set; }
        public int TotalNumber { get; set; }
        public double SinglePrice { get; set; }
        public float DiscountFloat { get; set; }
        public double DiscountAmount { get; set; }

        // Relationship
        public Book Book { get; set; }
        public Receipt Receipt { get; set; }

    }
}
