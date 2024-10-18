using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class AdminRegisterViewModel
    {
        public string Username { get; set; } // Add this line

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select a role.")]
        public string Role { get; set; }
    }
}
