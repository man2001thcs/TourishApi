using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data.Authentication;
using WebApplication1.Data.Connection;

namespace WebApplication1.Data
{
    public enum UserRole
    {
        New = 0, Staff = 1, Admin = 2,
    }

    [Table("User")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public UserRole Role { get; set; }
        public string Email { get; set; }
        public required string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public ICollection<RefreshToken> RefreshTokenList { get; set; }
        public ICollection<Message> MessageSentList { get; set; }
        public ICollection<Message> MessageReceiveList { get; set; }
        public ICollection<Notification> NotificationList { get; set; }

        public ICollection<NotificationCon> NotificationConList { get; set; }
        public ICollection<MessageCon> MessageConList { get; set; }

        public User()
        {
            this.Role = UserRole.New;
        }
    }
}
