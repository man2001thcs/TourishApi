using WebApplication1.Data;

namespace WebApplication1.Model
{

    public class TourishPlanModel
    {
        public Guid Id { get; set; }
        public required string GuestName { get; set; }
        public string? TourName { get; set; }
        public string StartingPoint { get; set; }
        public string EndPoint { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public PlanStatus PlanStatus { get; set; }
        public string? Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class TourishPlanInsertModel
    {
        public Guid Id { get; set; }
        public required string GuestName { get; set; }
        public string? TourName { get; set; }
        public string StartingPoint { get; set; }
        public string EndPoint { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public PlanStatus PlanStatus { get; set; }
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string EatingScheduleString { get; set; }
        public string MovingScheduleString { get; set; }
        public string StayingScheduleString { get; set; }
    }

    public class TourishPlanUpdateModel
    {
        public Guid Id { get; set; }
        public required string GuestName { get; set; }
        public string? TourName { get; set; }
        public string StartingPoint { get; set; }
        public string EndPoint { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public PlanStatus PlanStatus { get; set; }
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string EatingScheduleString { get; set; }
        public string MovingScheduleString { get; set; }
        public string StayingScheduleString { get; set; }


    }
}
