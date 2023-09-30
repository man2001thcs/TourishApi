using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data.RelationData;

namespace WebApplication1.Data
{
    public enum CoverMaterialEnum
    {
        Hard = 0, Soft = 1
    }

    public enum LanguageEnum
    {
        Others = 0, Vietnamese = 1, English = 2,
    }

    [Table("Book")]
    public class Book
    {
        [Key]
        public Guid id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }
        [Column(TypeName = "ntext")]
        public string? Description { get; set; }
        public int PageNumber { get; set; }
        public CoverMaterialEnum CoverMaterial { get; set; }
        public string BookSize { get; set; }
        public float BookWeight { get; set; }
        public LanguageEnum Language { get; set; }
        public int PublishYear { get; set; }
        public Guid? PublisherId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        // Relationship
        public BookStatus? BookStatus { get; set; }
        public ICollection<BookCategory> BookCategories { get; set; }
        public Publisher? Publisher { get; set; }
        public ICollection<BookAuthor> BookAuthors { get; set; }
        public ICollection<FullReceipt> FullReceiptList { get; set; }
        public ICollection<BookVoucher> BookVouchers { get; set; }
        public ICollection<SaveFile> Files { get; set; }

        public Book()
        {
            this.Language = LanguageEnum.Vietnamese;
            this.FullReceiptList = new List<FullReceipt>();
            this.Files = new List<SaveFile>();
        }


    }
}
