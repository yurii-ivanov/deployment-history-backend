using Newtonsoft.Json;

namespace DeploymentHistoryBackend.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RepositoryName { get; set; }
        public string OwnerName { get; set; }
        public string StoryRegEx { get; set; }

        [JsonIgnore]
        public bool IsDisabled{ get; set; }

        public IList<Deployment>? Deployments { get; set; }

        [JsonIgnore]
        public bool IsValid =>
            string.IsNullOrWhiteSpace(Name) == false &&
            string.IsNullOrWhiteSpace(RepositoryName) == false;
    }
}
