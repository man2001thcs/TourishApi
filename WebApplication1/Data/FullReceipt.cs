using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    [Table("FullReceipt")]
    public class FullReceipt
    {
        public Guid ReceiptId { get; set; }
        public Guid ProductId { get; set; }
        public int totalNumber { get; set; }
        public double singlePrice { get; set; }
        public float discountFloat { get; set; }
        public double discountAmount { get; set; }

        // Relationship
        public required Book Book { get; set; }
        public required Receipt Receipt { get; set; }

    }
}
