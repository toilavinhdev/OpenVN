using Microsoft.EntityFrameworkCore;
using SharedKernel.UnitOfWork;

namespace OpenVN.Infrastructure
{
    public class EfDbContext : DbContext, IUnitOfWork
    {
        private readonly string _connectionString;

        public EfDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public EfDbContext(DbContextOptions<EfDbContext> options) : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        //public DbSet<RolePermission> RolePermissions { get; set; }

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
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
