namespace CMCS.Models
{
    public class ProgrammeCoordinator
    {
        public int ProgrammeCoordinatorId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
