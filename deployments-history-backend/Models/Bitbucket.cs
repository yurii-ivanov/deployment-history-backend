using Newtonsoft.Json;

namespace DeploymentsHistoryBackend.Models
{
    public class BitbucketCommitResponse
    {
        [JsonProperty("values")]
        public IEnumerable<BitbucketCommit>? BitbucketCommits { get; set; }
    }
    public class BitbucketCommit
    {
        public string? Id { get; set; }
        public string? Message { get; set; }
    }
}
