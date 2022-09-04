using System.Linq;
using DeploymentHistoryBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DeploymentHistoryBackend.Data
{
    public interface IDeploymentsRepository
    {
        Task<IEnumerable<Deployment>> GetByAppId(int appId);
        Task<IEnumerable<Deployment>> GetByAppIdPaged(int appId, int skip, int take);
        Task<IEnumerable<Deployment>> Get(int limit);
        Task<Deployment> Save(Deployment deployment);
        Task<IEnumerable<Deployment>> SaveMany(IEnumerable<Deployment> deployments);
    }
    public class DeploymentsRepository : IDeploymentsRepository
    {
        private readonly DataContext _context;

        public DeploymentsRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Deployment>> GetByAppId(int appId)
        {
            return await _context.Deployments
                .Where(d => d.Application.Id == appId)
                .OrderByDescending(d => d.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<Deployment>> GetByAppIdPaged(int appId, int skip, int take)
        {
            return await _context.Deployments
                .Where(d => d.Application.Id == appId)
                .OrderByDescending(d => d.Timestamp)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Deployment>> Get(int limit)
        {
            return await _context.Deployments
                .OrderByDescending(d => d.Timestamp)
                .Take(limit)
                .Include(d => d.Application)
                .ToListAsync();
        }

        public async Task<Deployment> Save(Deployment deployment)
        {
            if (deployment.Application != null)
            {
                // Letting context know the Application Parent object already exists in DB
                // As it was obtained from cache it is considered a whole new object
                _context.Attach(deployment.Application);
                _context.Entry(deployment.Application).State = EntityState.Unchanged;
            }

            _context.Deployments.Add(deployment);
            await _context.SaveChangesAsync();
            return deployment;
        }

        public async Task<IEnumerable<Deployment>> SaveMany(IEnumerable<Deployment> deployments)
        {
            _context.Deployments.AddRange(deployments);
            await _context.SaveChangesAsync();
            return deployments;
        }
    }
}
