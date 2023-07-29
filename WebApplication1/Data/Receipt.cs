using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    public enum ReceiptStatus
    {
        Created = 0, Tranporting = 1, Completed = 2, Cancelled = 3,
    }
    [Table("Receipt")]
    public class Receipt
    {
        public Guid ReceiptId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public ReceiptStatus Status { get; set; }

        // Relationship
        public ICollection<FullReceipt> FullReceiptList { get; set; }
        public User User { get; set; }

        public Receipt()
        {
            this.FullReceiptList = new List<FullReceipt>();
        }
    }
}
