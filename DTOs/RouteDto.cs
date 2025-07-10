using System.ComponentModel.DataAnnotations;

namespace SmartSell.Api.DTOs
{
    public class RouteDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public int DriverId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int CurrentPassengers { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateRouteDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Origin { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Destination { get; set; } = string.Empty;

        [Required]
        public TimeSpan DepartureTime { get; set; }

        [Required]
        public TimeSpan ArrivalTime { get; set; }

        [Required]
        public int DriverId { get; set; }

        [Range(1, 100)]
        public int Capacity { get; set; } = 40;

        [Required]
        public DateTime RouteDate { get; set; }
    }

    public class UpdateRouteDto
    {
        [MaxLength(150)]
        public string? Name { get; set; }

        [MaxLength(200)]
        public string? Origin { get; set; }

        [MaxLength(200)]
        public string? Destination { get; set; }

        public TimeSpan? DepartureTime { get; set; }

        public TimeSpan? ArrivalTime { get; set; }

        public int? DriverId { get; set; }

        [Range(1, 100)]
        public int? Capacity { get; set; }

        public DateTime? RouteDate { get; set; }

        public string? Status { get; set; }
    }
}
