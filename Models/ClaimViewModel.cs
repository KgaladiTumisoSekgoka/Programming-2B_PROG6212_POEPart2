using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class ClaimViewModel
    {
        [Required]
        public int ClaimID { get; set; } // The unique claim identifier

        [Required]
        public DateTime SubmissionDate { get; set; } = DateTime.Now; // Submission date of the claim

        [Required]
        public string Month { get; set; } // The claimed month

        [Required]
        [Range(0, double.MaxValue)]
        public decimal HoursWorked { get; set; } // Hours worked

        [Required]
        [Range(0, double.MaxValue)]
        public decimal HourlyRate { get; set; } // Hourly rate

        public decimal TaxDeductions { get; set; } = 0; // Tax deductions, defaulting to 0

        public decimal TotalClaim { get; set; } // Ensure this property exists

        public IFormFile Document { get; set; }


        public string? DocumentPath { get; set; } // Path to the uploaded document

        public string Status { get; set; } = "Pending"; // Claim status (default to Pending)

        // Add this property for the lecturer ID
        public int LecturerId { get; set; } // Foreign key linking to Lecturer

        /*public static implicit operator ClaimViewModel(ClaimViewModel v)
        {
            throw new NotImplementedException();
        }*/
    }
}
