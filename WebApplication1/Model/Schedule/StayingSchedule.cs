﻿using WebApplication1.Data.Schedule;

namespace WebApplication1.Model.Schedule
{
    public enum RestHouseType
    {
        HomeStay = 0, Hotel = 1
    }
    public class StayingScheduleModel
    {
        public Guid Id { get; set; }
        public string? PlaceName { get; set; }
        public string? Address { get; set; }
        public string? SupportNumber { get; set; }
        public double? SinglePrice { get; set; }
        public RestHouseType RestHouseType { get; set; }
        public Guid RestHouseBranchId { get; set; }
        public StayingScheduleStatus Status { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
