
namespace CMCS.Models
{
    public class Lecturer
    {
        public int LecturerId { get; set; }
        public string Department { get; set; }
        public int UserId { get; set; }  // Foreign key to the Users table

        // Navigation property for Claims
        public ICollection<Claim> Claims { get; set; }
    }
}
