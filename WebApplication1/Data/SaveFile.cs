using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data.Chat;

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
        public UserMessage? Message { get; set; }
        public TourishPlan? TourishPlan { get; set; }
    }
}
