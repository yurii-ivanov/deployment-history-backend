using System.ComponentModel.DataAnnotations;

namespace DeploymentHistoryBackend.Controllers.Models
{
    public class DeploymentEdit
    {
        [Required]
        public string CommitId { get; set; }
        [Required]
        public string ApplicationName { get; set; }
        public string BranchName { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
