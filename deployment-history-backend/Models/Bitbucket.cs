using Newtonsoft.Json;

namespace DeploymentHistoryBackend.Models
{
    public class BitbucketCommitResponse
    {
        [JsonProperty("values")]
        public IEnumerable<SourceControlCommit>? Commits { get; set; }
    }
    public class SourceControlCommit
    {
        public string? Id { get; set; }
        public string? Message { get; set; }
    }
}
