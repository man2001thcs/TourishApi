using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data.Chat;

namespace WebApplication1.Data.Connection
{
    [Table("GuestMessageCon")]
    public class GuestMessageCon
    {
        [Key]
        public Guid Id { get; set; }
        public required string GuestName { get; set; }
        public required string GuestEmail { get; set; }
        public required string GuestPhoneNumber { get; set; }
        public Guid AdminId { get; set; }
        public string ConnectionID { get; set; }
        public string UserAgent { get; set; }
        public bool Connected { get; set; }
        public DateTime CreateDate { get; set; }

        // Relationship
        public User Admin { get; set; }

        public ICollection<GuestMessage>? GuestMessages { get; set; }
    }
}
