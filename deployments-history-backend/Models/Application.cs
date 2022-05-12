using Newtonsoft.Json;

namespace DeploymentsHistoryBackend.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RepoUrl { get; set; }
        public string StoryRegEx { get; set; }
        public string BranchName { get; set; }

        [JsonIgnore]
        public bool IsDisabled{ get; set; }

        public IList<Deployment>? Deployments { get; set; }

        [JsonIgnore]
        public bool IsValid =>
            string.IsNullOrWhiteSpace(Name) == false &&
            string.IsNullOrWhiteSpace(RepoUrl) == false;
    }
}
