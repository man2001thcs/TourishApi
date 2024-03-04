using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data.Connection
{
    [Table("GuestMessageCon")]
    public class GuestMessageCon
    {
        [Key]
        public Guid Id { get; set; }
        public required string guestName { get; set; }
        public required string guestEmail { get; set; }
        public required string guestPhoneNumber { get; set; }
        public Guid AdminId { get; set; }
        public string ConnectionID { get; set; }
        public string UserAgent { get; set; }
        public bool Connected { get; set; }
        public DateTime CreateDate { get; set; }

        // Relationship
        public User Admin { get; set; }
    }
}
