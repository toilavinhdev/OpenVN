using Microsoft.EntityFrameworkCore;
using SharedKernel.UnitOfWork;

namespace OpenVN.BackgroundJobs
{
    public class BackgroundDbContext : DbContext, IUnitOfWork
    {
        private readonly string _connectionString;
        private readonly ILoggerFactory _loggerFactory;

        public BackgroundDbContext(string connectionString, ILoggerFactory loggerFactory)
        {
            _connectionString = connectionString;
            _loggerFactory = loggerFactory;
        }

        public BackgroundDbContext(DbContextOptions<BackgroundDbContext> options) : base(options)
        {
        }

        public DbSet<JobTenant> Tenants { get; set; }

        public DbSet<JobUser> Users { get; set; }

        public DbSet<Process> Processes { get; set; }


        public async Task CommitAsync(bool dispatch = false, CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync(cancellationToken);
        }

        public Task RollBackAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
            //optionsBuilder.LogTo(s => System.Diagnostics.Debug.WriteLine(s))
            //              .EnableDetailedErrors(true)
            //              .EnableSensitiveDataLogging(true);
            //optionsBuilder.UseLoggerFactory(_loggerFactory);
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
