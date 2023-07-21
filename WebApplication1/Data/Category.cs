using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public virtual ICollection<Book>? BookList { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
