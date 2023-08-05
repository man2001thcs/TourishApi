using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data.RelationData;

namespace WebApplication1.Data
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public required string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        // Relationship
        public ICollection<BookCategory> BookCategories { get; set; }
    }
}
