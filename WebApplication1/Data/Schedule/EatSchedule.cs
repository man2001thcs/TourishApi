using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data.Schedule
{
    public enum EatScheduleStatus
    {
        Created = 0, OnGoing = 1, Completed = 2, Cancelled = 3
    }

    [Table("EatSchedule")]
    public class EatSchedule
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TourishPlanId { get; set; }

        [Required]
        [MaxLength(200)]
        public string? PlaceName { get; set; }
        public string? Address { get; set; }
        public string? SupportNumber { get; set; }
        public Guid RestaurantId { get; set; }
        public double? SinglePrice { get; set; }
        public EatScheduleStatus Status { get; set; }
        [Required]
        [Column(TypeName = "ntext")]
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public TourishPlan TourishPlan { get; set; }
    }
}
