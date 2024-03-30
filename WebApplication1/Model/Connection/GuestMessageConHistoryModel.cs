using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model.Connection
{
    public class GuestMessageConHistoryModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid GuestConId { get; set; }
        public Guid AdminConId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
