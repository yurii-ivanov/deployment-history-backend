namespace DeploymentHistoryBackend.Models
{
    public class Release
    {
        public string CommitId { get; set; }
        public IEnumerable<string> Stories { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
