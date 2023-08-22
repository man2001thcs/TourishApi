namespace WebApplication1.Model
{
    public class BookStatusModel
    {
        public Guid ProductId { get; set; }
        public int SoldNumberInMonth { get; set; }
        public int TotalSoldNumber { get; set; }
        public int RemainNumber { get; set; }
        public double CurrentPrice { get; set; }
    }
}
