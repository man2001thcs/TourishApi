using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    [Table("Notification")]
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserCreateId { get; set; }
        public Guid? UserReceiveId { get; set; }
        public string Content { get; set; }
        public Boolean IsRead { get; set; }
        public Boolean IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        // Relationship
        public User UserCreator { get; set; }
        public User UserReceiver { get; set; }
    }
}
