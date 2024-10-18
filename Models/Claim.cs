using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    public class Claim
    {
        [Key]
        public int ClaimID { get; set; } // Primary key

        [Required]
        public DateTime SubmissionDate { get; set; } = DateTime.Now; // Date the claim is submitted

        [Required]
        [StringLength(20)]
        public string Month { get; set; } // Month being claimed (e.g., "September 2023")

        [Required]
        [Range(0, double.MaxValue)]
        public decimal HoursWorked { get; set; } // Number of hours worked

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal HourlyRate { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TaxDeductions { get; set; } = 0; // Optional tax deductions

        [Required]
        [StringLength(20)]
        public string? Status { get; set; } = "Pending"; // Claim status

        public int? LecturerID { get; set; } // Foreign key linking to Lecturer
        public Lecturer Lecturer { get; set; } // Navigation property to Lecturer

        // Add this property for the reason of approval/rejection
        public string? Reason { get; set; } // Optional reason for rejection/approval
        public decimal TotalClaim { get; set; } // Ensure this property exists

        public string? DocumentPath { get; set; } // Path to the uploaded document
        public int? AdminId { get; set; }  // Foreign key to Admin table
        public Admin Admin { get; set; }  // Navigation property for Admin
    }
}
