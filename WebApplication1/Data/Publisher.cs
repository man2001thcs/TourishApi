using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    [Table("Publisher")]
    public class Publisher
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string PublisherName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        [Required]
        [MaxLength(200)]
        public string? Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        // Relationship
        public ICollection<Book> BookList { get; set; }
    }
}
