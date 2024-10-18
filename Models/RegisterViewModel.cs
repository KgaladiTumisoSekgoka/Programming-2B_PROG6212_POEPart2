namespace CMCS.Models
{
    public class RegisterViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // 'Lecturer'
        public string? Departments { get; set; } // New property to capture departments
    }
}

