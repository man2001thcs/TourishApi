using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    [Table("Message")]
    public class Message
    {
        public Guid Id { get; set; }
        public required Guid UserSentId { get; set; }
        public Guid UserReceiveId { get; set; }
        public Guid? GroupId { get; set; }
        public string Content { get; set; }
        public Boolean IsRead { get; set; }
        public Boolean IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        // Relationship
        public User UserSent { get; set; }
        public User UserReceive { get; set; }
        public ICollection<SaveFile>? Files { get; set; }

        public Message()
        {
            this.Content = "";
            this.IsRead = false;
            this.IsDeleted = false;
        }
    }
}
