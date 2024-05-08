using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data.Receipt;

namespace WebApplication1.Data
{
    [Table("ServiceSchedule")]
    public class ServiceSchedule
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public ScheduleType ScheduleType { get; set; }   
        public PlanStatus PlanStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public FullReceipt? FullReceipt { get; set; }
    }
}
