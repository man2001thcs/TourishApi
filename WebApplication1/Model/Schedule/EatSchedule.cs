﻿namespace WebApplication1.Model.Schedule
{
    public class EatScheduleModel
    {
        public Guid Id { get; set; }
        public Guid TourishPlanId { get; set; }
        public string? PlaceName { get; set; }
        public string? Address { get; set; }
        public string? SupportNumber { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
