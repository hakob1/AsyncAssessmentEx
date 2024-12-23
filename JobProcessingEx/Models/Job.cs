namespace JobProcessingEx.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public bool Processed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
