using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    public enum InterestStatus
    {
        Creator = 0,
        Modifier = 1,
        Interest = 2,
        NotInterested = 3,
    }

    [Table("TourishInterest")]
    public class TourishInterest
    {
        public Guid Id { get; set; }
        public Guid TourishPlanId { get; set; }
        public Guid UserId { get; set; }
        public InterestStatus InterestStatus { get; set; }
        public DateTime UpdateDate { get; set; }
<<<<<<< HEAD
        public TourishPlan TourishPlan { get; set; }
=======
>>>>>>> a3c0c39 (Add migration)
        public User User { get; set; }
        public TourishPlan? TourishPlan { get; set; }
    }
}
