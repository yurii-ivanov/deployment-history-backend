using DeploymentHistoryBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DeploymentHistoryBackend.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Deployment> Deployments { get; set; }
    }
}
