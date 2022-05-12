using System.Net;
using DeploymentsHistoryBackend.Models;
using DeploymentsHistoryBackend.Models.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DeploymentsHistoryBackend.Data
{
    public interface IBitbucketRepository
    {
        Task<IEnumerable<BitbucketCommit>> GetCommits(string repoName, string projectName, string branchName);
    }
    public class BitbucketRepository : IBitbucketRepository
    {
        private readonly BitbucketConfig _bitbucketConfig;
        private readonly HttpClient _httpClient;
        public BitbucketRepository(IHttpClientFactory httpClientFactory, IOptions<BitbucketConfig> options)
        {
            _bitbucketConfig = options.Value;
            _httpClient = httpClientFactory.CreateClient("Any");
        }

        public async Task<IEnumerable<BitbucketCommit>> GetCommits(string repoName, string projectName, string branchName)
        {
            if (string.IsNullOrWhiteSpace(repoName))
            {
                throw new ArgumentException(nameof(repoName) + " is missing");
            }
            var message = new HttpRequestMessage
            {
                RequestUri = new Uri($"{_bitbucketConfig.BitbucketHostUrl}/rest/api/1.0/projects/{projectName}/repos/{repoName}/commits/?until={branchName}&limit=50")
            };

            message.Headers.Add("Authorization", $"Bearer {_bitbucketConfig.BitbucketAccessToken}");
            var response = await _httpClient.SendAsync(message);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException(
                    $"BitbucketRepository: error while getting commits(return code: {response.StatusCode})");
            }

            var data = await response.Content.ReadAsStringAsync();

            var commitsResponse = JsonConvert.DeserializeObject<BitbucketCommitResponse>(data);
            return commitsResponse.BitbucketCommits;
        }
    }
}
