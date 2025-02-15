﻿namespace CMCS.Models
{
    public class User
    {
        public int UserId { get; set; }  
        public string Username { get; set; }  
        public string Password { get; set; }  
        public string Email { get; set; }
        public string? Departments { get; set; }
        public string Role { get; set; } // 'Lecturer', 'Programme Coordinator', 'Academic Manager'
        //public bool IsApproved { get; set; }
    }
}
