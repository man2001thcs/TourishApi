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

        public string? GoogleToken { get; set; }
    }

    public class UserUpdateModel
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }

    public class UserUpdatePasswordModel
    {
        public string? UserName { get; set; }
        public string NewPassword { get; set; }
    }
}
