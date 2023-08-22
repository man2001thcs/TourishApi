using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model
{
    public class UserModel
    {
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        public required string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
