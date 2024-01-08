using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data.Receipt;
using WebApplication1.Data.Schedule;

namespace WebApplication1.Data
{
    public enum PlanStatus
    {
        Waiting = 0, ConfirmInfo = 1, OnGoing = 2, Complete = 3, Cancel = 4
    }

    [Table("TourishPlan")]
    public class TourishPlan
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? TourName { get; set; }
        public string StartingPoint { get; set; }
        public string EndPoint { get; set; }
        public string SupportNumber { get; set; }
        public int TotalTicket { get; set; }
        public int RemainTicket { get; set; }
        public PlanStatus PlanStatus { get; set; }

        [Required]
        [Column(TypeName = "ntext")]
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
