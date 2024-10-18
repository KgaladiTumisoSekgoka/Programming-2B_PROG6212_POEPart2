namespace CMCS.Models
{
    public class Approval
    {
        public int approvalId { get; set; }
        public DateTime approvalDate { get; set; }
        public string status { get; set; }
        public string comments { get; set; }
        public string ClaimId { get; set; }
        public string AdminId { get; set; }
    }
}
