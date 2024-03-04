using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data;

namespace TourishApi.Data.Chat
{
    [Table("Message")]
    public class Message
    {
        public Guid Id { get; set; }
        public required Guid UserSentId { get; set; }
        public Guid UserReceiveId { get; set; }
        public Guid? GroupId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        // Relationship
        public User UserSent { get; set; }
        public User UserReceive { get; set; }
        public ICollection<SaveFile>? Files { get; set; }

        public Message()
        {
            Content = "";
            IsRead = false;
            IsDeleted = false;
        }
    }
}
