using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    [Table("TourishSchedule")]
    public class TourishSchedule
    {
        public Guid Id { get; set; }
        public Guid TourishPlanId { get; set; }
        public PlanStatus PlanStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public TourishPlan TourishPlan { get; set; }
    }
}
