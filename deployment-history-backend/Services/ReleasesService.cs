using System.Text.RegularExpressions;
using DeploymentHistoryBackend.Controllers;
using DeploymentHistoryBackend.Data;
using DeploymentHistoryBackend.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace DeploymentHistoryBackend.Services
{
    public interface IReleasesService
    {
        Task<IEnumerable<Release>> GetReleases(Application app);
    }
    public class ReleasesService : IReleasesService
    {
        private readonly IBitbucketRepository _bitbucketRepository;

        public ReleasesService(IBitbucketRepository bitbucketRepository)
        {
            _bitbucketRepository = bitbucketRepository;
        }

        public async Task<IEnumerable<Release>> GetReleases(Application app)
        {
            var bb = app.RepoUrl.Split('/');
            var bbRepoName = bb[6];
            var bbProjectName = bb[4];
             var allCommits = (await _bitbucketRepository.GetCommits(bbRepoName, bbProjectName)).ToList();

            var releases = new List<Release>();
            var length = app.Deployments.Count() - 2;
            for (var i = 0; i <= length; i++)
            {
                var commitTo = app.Deployments[i].CommitId;
                var commitFrom = app.Deployments[i+1].CommitId;
                var commits = GetCommitsBetween(allCommits, commitTo, commitFrom).ToList();
                var stories = GetReleasedStories(commits, app.StoryRegEx);

                stories  = stories
                    .GroupBy(s => s.ToUpperInvariant())
                    .Select(g => g.Key)
                    .OrderByDescending(s => s);
                //stories = stories.Distinct();

                releases.Add(new Release()
                {
                    CommitId = app.Deployments[i].CommitId,
                    Timestamp = app.Deployments[i].Timestamp,
                    Stories = stories
                });
            }

            return releases;
        }


        private IEnumerable<string> GetReleasedStories(List<BitbucketCommit> commits, string regEx)
        {
            var storyRegEx = new Regex(regEx);

            var storyIds = new List<string>();
            foreach (var commit in commits)
            {
                //storyIds.AddRange(storyRegEx.Matches(commit.Message).SelectMany(m => m.Value));
                storyIds.AddRange(storyRegEx.Matches(commit.Message).Select(m => m.Value));
            }

            return storyIds;
        }
        private IEnumerable<BitbucketCommit> GetCommitsBetween(List<BitbucketCommit> commits, string commitTo, string commitFrom)
        {
            var list = new List<BitbucketCommit>();

            var found = false;
            foreach (var commit in commits)
            {
                if (found)
                {
                    if (commit.Id == commitFrom)
                    {
                        break;
                    }
                    list.Add(commit);
                }

                if (commit.Id == commitTo)
                {
                    found = true;
                    list.Add(commit);
                }
            }

            return list;
        }
    }
}
