using System.ComponentModel.DataAnnotations;

namespace SmartSell.Api.DTOs
{
    public class DriverDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Cnh { get; set; } = string.Empty;
        public string Vehicle { get; set; } = string.Empty;
        public DateTime LicenseExpiry { get; set; }
        public string Status { get; set; } = "active";
        public DateTime CreatedAt { get; set; }
    }

    public class CreateDriverDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string Cnh { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Vehicle { get; set; } = string.Empty;

        [Required]
        public DateTime LicenseExpiry { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateDriverDto
    {
        [MaxLength(150)]
        public string? Name { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [StringLength(11, MinimumLength = 11)]
        public string? Cnh { get; set; }

        [MaxLength(100)]
        public string? Vehicle { get; set; }

        public DateTime? LicenseExpiry { get; set; }
    }
}
