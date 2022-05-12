using System.ComponentModel.DataAnnotations;

namespace DeploymentsHistoryBackend.Controllers.Models
{
    public class DeploymentEdit
    {
        [Required]
        public string CommitId { get; set; }
        [Required]
        public string ApplicationName { get; set; }
        public DateTime? Timestamp { get; set; }
        public long? Milliseconds { get; set; }
    }
}
