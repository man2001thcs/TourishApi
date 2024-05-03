﻿using WebApplication1.Data;

namespace WebApplication1.Model
{
    public class ScheduleInterestModel
    {
        public Guid? Id { get; set; }
        public Guid ScheduleId { get; set; }
        public Guid UserId { get; set; }
        public InterestStatus InterestStatus { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
