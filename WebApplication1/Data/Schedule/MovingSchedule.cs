﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data.Transport;

namespace WebApplication1.Data.Schedule
{
    public enum MovingScheduleStatus
    {
        Created = 0, OnGoing = 1, Completed = 2, Cancelled = 3
    }

    [Table("MovingSchedule")]
    public class MovingSchedule
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? TourishPlanId { get; set; }
        public string? Name { get; set; }

        [Required]
        [MaxLength(200)]
        public string? DriverName { get; set; }
        public string? VehiclePlate { get; set; }
        public string? PhoneNumber { get; set; }
        public string? BranchName { get; set; }

        public VehicleType VehicleType { get; set; }
        public Guid TransportId { get; set; }
        public double? SinglePrice { get; set; }

        public string StartingPlace { get; set; }
        public string HeadingPlace { get; set; }
        public MovingScheduleStatus Status { get; set; }
        [Column(TypeName = "ntext")]
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public TourishPlan TourishPlan { get; set; }
        public ICollection<ScheduleInterest>? ScheduleInterestList { get; set; }
        public ICollection<Notification>? NotificationList { get; set; }
    }
}
