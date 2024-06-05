namespace WebApplication1.Model.VirtualModel
{
    public class Response
    {
        public int resultCd { get; set; }
        public string? Error { get; set; }
        public string? MessageCode { get; set; }

        public object? Data { get; set; }

        public int? AveragePoint { get; set; }

        public Guid? returnId { get; set; }
        public int? curId { get; set; }
        public string? type { get; set; }

        public int? count { get; set; }
    }
}
