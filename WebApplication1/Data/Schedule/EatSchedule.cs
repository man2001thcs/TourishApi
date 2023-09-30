﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{

    [Table("EatSchedule")]
    public class EatSchedule
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(70)]
        public string? PlaceName { get; set; }
        public string? Address { get; set; }
        public string? SupportNumber { get; set; }

        public int TotalTicket { get; set; }
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
