﻿using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data.Schedule;

namespace WebApplication1.Data.Receipt
{
    public enum ReceiptStatus
    {
        Created = 0, OnGoing = 1, Completed = 2, Cancelled = 3,
    }

    [Table("TotalReceipt")]
    public class TotalReceipt
    {
        public Guid TotalReceiptId { get; set; }
        public Guid? TourishPlanId { get; set; }
        public Guid? MovingScheduleId { get; set; }
        public Guid? StayingScheduleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public string? Description { get; set; }
        public ReceiptStatus Status { get; set; }

        // Relationship
        public TourishPlan? TourishPlan { get; set; }
        public MovingSchedule? MovingSchedule { get; set; }
        public StayingSchedule? StayingSchedule { get; set; }
        public ICollection<FullReceipt> FullReceiptList { get; set; }

        public TotalReceipt()
        {
            this.FullReceiptList = new List<FullReceipt>();
        }
    }
}
