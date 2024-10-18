namespace CMCS.Models
{
    public class AcademicManager
    {
        public int AcademicManagerId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
