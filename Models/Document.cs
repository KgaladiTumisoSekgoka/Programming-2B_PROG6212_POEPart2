namespace CMCS.Models
{
    public class Document
    {
        public int DocumentId { get; set; }
        public string documentName { get; set; }
        public string documentType { get; set; }
        public string filePath { get; set; }
        public string ClaimId { get; set; }
    }
}
