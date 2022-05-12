using DeploymentsHistoryBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DeploymentsHistoryBackend.Data
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
