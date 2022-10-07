using System.Linq;
using DeploymentHistoryBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DeploymentHistoryBackend.Data
{
    public interface IApplicationsRepository
    {
        Task<Application> GetById(int appId);
        Task<Application> GetByName(string name);
        Task<IEnumerable<Application>> GetAll();
        Task<Application> Save(Application application);
        Task<bool> Delete(int appId);
    }
    public class ApplicationsRepository : IApplicationsRepository
    {
        private readonly DataContext _context;

        public ApplicationsRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<Application> GetById(int appId)
        {
            if (appId < 0)
            {
                throw new ArgumentException(nameof(appId) + " is incorrect " + appId);
            }

            Application app;

            try
            {
                app = await _context.Applications.FirstAsync(a => a.Id == appId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return app;
        }

        public async Task<Application> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name) + " is missing");
            }

            Application app;

            try
            {
                app = await _context.Applications.FirstAsync(a => a.Name == name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return app;
        }

        public async Task<IEnumerable<Application>> GetAll()
        {
            var query = _context.Applications
                .OrderBy(a => a.Name).Where(a => a.IsDisabled == false);

            var apps = await query.ToListAsync();
            return apps;
        }

        public async Task<Application> Save(Application application)
        {
            if (application.IsValid == false)
            {
                throw new ArgumentException(nameof(application) + $" is not valid + ({application})");
            }

            if (string.IsNullOrWhiteSpace(application.StoryRegEx))
            {
                application.StoryRegEx = "[A-Z]{1,10}-[0-9]+"; // default matching AAA-1234
            }

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task<bool> Delete(int appId)
        {
            if(appId < 0)
            {
                throw new ArgumentException(nameof(appId) + $" is not valid + ({appId})");
            }
            var application = await _context.Applications.FirstOrDefaultAsync(app => app.Id == appId);
            if (application == null)
            {
                return true;
            }

            application.IsDisabled = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
