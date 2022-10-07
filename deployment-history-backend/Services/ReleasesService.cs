using System.Text.RegularExpressions;
using DeploymentHistoryBackend.Data;
using DeploymentHistoryBackend.Models;

namespace DeploymentHistoryBackend.Services
{
    public interface IReleasesService
    {
        Task<IEnumerable<Release>> GetReleases(Application app);
    }
    public class ReleasesService : IReleasesService
    {
        private readonly ISourceControlRepository _sourceControlRepository;

        public ReleasesService(ISourceControlRepository sourceControlRepository)
        {
            _sourceControlRepository = sourceControlRepository;
        }

        public async Task<IEnumerable<Release>> GetReleases(Application app)
        {
            var allCommits = (
                await _sourceControlRepository.GetCommits(app.RepositoryName, app.OwnerName)
                ).ToList();

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

                releases.Add(new Release()
                {
                    CommitId = app.Deployments[i].CommitId,
                    Timestamp = app.Deployments[i].Timestamp,
                    Stories = stories
                });
            }

            return releases;
        }


        private IEnumerable<string> GetReleasedStories(List<SourceControlCommit> commits, string regEx)
        {
            var storyRegEx = new Regex(regEx);

            var storyIds = new List<string>();
            foreach (var commit in commits)
            {
                storyIds.AddRange(storyRegEx.Matches(commit.Message).Select(m => m.Value));
            }

            return storyIds;
        }
        private IEnumerable<SourceControlCommit> GetCommitsBetween(List<SourceControlCommit> commits, string commitTo, string commitFrom)
        {
            var list = new List<SourceControlCommit>();

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
