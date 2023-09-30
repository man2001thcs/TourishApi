using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    public enum ResourceTypeEnum
    {
        Avatar = 0, Product = 1, Message = 2
    }

    [Table("SaveFile")]
    public class SaveFile
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AccessSourceId { get; set; }
        public ResourceTypeEnum ResourceType { get; set; }
        public required string FileType { get; set; }
        public DateTime CreatedDate { get; set; }
        public Message? Message { get; set; }
        public Book? Book { get; set; }
    }
}
