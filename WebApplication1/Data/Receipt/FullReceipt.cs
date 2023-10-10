using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data.Receipt
{
    public enum FullReceiptStatus
    {
        Created = 0, AwaitPayment = 1, Completed = 2, Cancelled = 3,
    }

    [Table("FullReceipt")]
    public class FullReceipt
    {
        public Guid FullReceiptId { get; set; }
        public Guid TotalReceiptId { get; set; }
        public string GuestName { get; set; }
        public double OriginalPrice { get; set; }
        public int TotalTicket { get; set; }
        [Required]
        [MaxLength(900)]
        public FullReceiptStatus Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public float DiscountFloat { get; set; }
        public double DiscountAmount { get; set; }

        // Relationship
        public TotalReceipt TotalReceipt { get; set; }

        public FullReceipt()
        {
            TotalTicket = 1;
        }

    }
}
