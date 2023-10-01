using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data.Receipt;
using WebApplication1.Data.Schedule;

namespace WebApplication1.Data
{
    public enum PlanStatus
    {
        Waiting = 0, ConfirmInfo = 1, Paid = 2, OnGoing = 3, Complete = 4, Repaid = 5, Cancel = 6
    }

    [Table("TourishPlan")]
    public class TourishPlan
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string GuestName { get; set; }
        public string? TourName { get; set; }
        public string StartingPoint { get; set; }
        public string EndPoint { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public PlanStatus PlanStatus { get; set; }
        [Required]
        [MaxLength(200)]
        public string? Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public TotalReceipt TotalReceipt { get; set; }
        public ICollection<EatSchedule> EatSchedules { get; set; }
        public ICollection<MovingSchedule> MovingSchedules { get; set; }
        public ICollection<StayingSchedule> StayingSchedules { get; set; }

        public TourishPlan()
        {
            PlanStatus = PlanStatus.Waiting;
            EatSchedules = new List<EatSchedule>();
            MovingSchedules = new List<MovingSchedule>();
            StayingSchedules = new List<StayingSchedule>();
        }
    }
}
