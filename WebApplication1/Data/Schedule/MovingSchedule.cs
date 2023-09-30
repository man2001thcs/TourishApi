using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data.Schedule
{
    public enum VehicleType
    {
        PassengerCar = 0, Plane = 1, Train = 2
    }

    [Table("MovingSchedule")]
    public class MovingSchedule
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(70)]
        public string? DriverName { get; set; }
        public string? VehiclePlate { get; set; }
        public string? PhoneNumber { get; set; }

        public VehicleType VehicleType { get; set; }
        public Guid TransportId { get; set; }

        public string StartingPlace { get; set; }
        public string HeadingPlace { get; set; }

        public int TotalTicket { get; set; }
        public string TicketType { get; set; }
        public double SubPrice { get; set; }
        public double TotalPrice { get; set; }

        [Required]
        [MaxLength(600)]
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
