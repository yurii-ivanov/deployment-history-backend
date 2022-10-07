using System.Net;
using DeploymentHistoryBackend.Models;
using DeploymentHistoryBackend.Models.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DeploymentHistoryBackend.Data
{
    public interface ISourceControlRepository
    {
        Task<IEnumerable<SourceControlCommit>> GetCommits(string repoName, string projectName, string branchName = "master");
    }


    public class BitbucketRepository : ISourceControlRepository
    {
        private readonly HttpClient _httpClient;
        private const string API_URL = "rest/api/1.0/projects";
        public BitbucketRepository(IHttpClientFactory httpClientFactory, IOptions<BitbucketConfig> options)
        {
            _httpClient = httpClientFactory.CreateClient(Constants.BITBUCKET_CLIENT_NAME);
        }

        public async Task<IEnumerable<SourceControlCommit>> GetCommits(string repoName, string projectName, string branchName = "master")
        {
            if (string.IsNullOrWhiteSpace(repoName))
            {
                throw new ArgumentException(nameof(repoName) + " is missing");
            }

            var uri = $"/{API_URL}/{projectName}/repos/{repoName}/commits/?until={branchName}&limit=50";
            var response = await _httpClient.GetAsync(uri);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException(
                    $"BitbucketRepository: error while getting commits(return code: {response.StatusCode})");
            }

            var data = await response.Content.ReadAsStringAsync();

            var commitsResponse = JsonConvert.DeserializeObject<BitbucketCommitResponse>(data);
            return commitsResponse.Commits;
        }
    }
}
