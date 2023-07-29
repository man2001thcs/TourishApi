using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    [Table("Author")]
    public class Author
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        [Required]
        [MaxLength(200)]
        public string? Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        // Relationship
        public ICollection<BookAuthor> BookAuthors { get; set; }
    }
}
