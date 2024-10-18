namespace CMCS.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }   // Navigation property for the related user
    }
}
