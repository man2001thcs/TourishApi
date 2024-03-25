namespace WebApplication1.Model
{
    public class NotificationModel
    {
        public Guid? Id { get; set; }
        public Guid UserCreateId { get; set; }
        public Guid? UserReceiveId { get; set; }
        public string Content { get; set; }
        public string? ContentCode { get; set; }
        public Guid? TourishPlanId { get; set; }
        public Boolean IsRead { get; set; }
        public Boolean IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public class NotificationDTOModel
    {
        public Guid? Id { get; set; }
        public Guid UserCreateId { get; set; }
        public Guid? UserReceiveId { get; set; }
        public string Content { get; set; }
        public string? ContentCode { get; set; }
        public String? TourName { get; set; }
        public String? CreatorFullName { get; set; }
        public Boolean IsRead { get; set; }
        public Boolean IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
