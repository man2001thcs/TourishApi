using WebApplication1.Data;

namespace WebApplication1.Model
{
    public class BookModel
    {
        public Guid id { get; set; }
        public string Title { get; set; }
        public int PageNumber { get; set; }
        public string Description { get; set; }
        public CoverMaterialEnum CoverMaterial { get; set; }
        public LanguageEnum Language { get; set; }
        public string BookSize { get; set; }
        public float BookWeight { get; set; }
        public int PublishYear { get; set; }
        public Guid PublisherId { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }

    public class BookInsertModel
    {
        public Guid id { get; set; }
        public Guid PublisherId { get; set; }
        public string Title { get; set; }
        public int PageNumber { get; set; }
        public string Description { get; set; }
        public CoverMaterialEnum CoverMaterial { get; set; }
        public LanguageEnum Language { get; set; }
        public string BookSize { get; set; }
        public float BookWeight { get; set; }
        public int PublishYear { get; set; }
        public int SoldNumberInMonth { get; set; }
        public int TotalSoldNumber { get; set; }
        public int RemainNumber { get; set; }
        public double CurrentPrice { get; set; }
        public string CategoryRelationString { get; set; }
        public string AuthorRelationString { get; set; }
        public string VoucherRelationString { get; set; }
    }

    public class BookUpdateModel
    {
        public Guid id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? PageNumber { get; set; }
        public Guid? PublisherId { get; set; }

        public CoverMaterialEnum CoverMaterial { get; set; }
        public string BookSize { get; set; }
        public float BookWeight { get; set; }
        public int PublishYear { get; set; }
        public LanguageEnum Language { get; set; }

        // Relationship
        public BookStatus? BookStatus { get; set; }
        public string? CategoryRelationString { get; set; }
        public string? AuthorRelationString { get; set; }
        public string? VoucherRelationString { get; set; }
    }
}
