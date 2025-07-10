using System.ComponentModel.DataAnnotations;

namespace SmartSell.Api.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Cpf { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = "active";
        public DateTime CreatedAt { get; set; }
    }

    public class CreateStudentDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(14)]
        public string Cpf { get; set; } = string.Empty;

        public string? Route { get; set; }
        public DateTime? EnrollmentDate { get; set; }
    }

    public class UpdateStudentDto
    {
        [MaxLength(150)]
        public string? Name { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(14)]
        public string? Cpf { get; set; }

        public string? Route { get; set; }
        public DateTime? EnrollmentDate { get; set; }
    }
}
