using WebApplication1.Data;

namespace WebApplication1.Model
{
    public class TourishCommentModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public User User { get; set; }

    }
}
