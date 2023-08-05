namespace WebApplication1.Model
{
    public class VoucherModel
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public float DiscountFloat { get; set; }
        public double DiscountAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
