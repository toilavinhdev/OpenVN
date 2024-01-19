using Microsoft.EntityFrameworkCore;
using SharedKernel.Application;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.Log;

namespace OpenVN.BackgroundJobs
{
    public class ProcessBackgroundJob : BackgroundService
    {
        private readonly ISequenceCaching _sequenceCaching;
        private readonly ITenantReadOnlyRepository _tenantRepository;
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public ProcessBackgroundJob(
            ISequenceCaching sequenceCaching,
            ITenantReadOnlyRepository tenantRepository,
            IConfiguration configuration,
            ILoggerFactory loggerFactory
        )
        {
            _sequenceCaching = sequenceCaching;
            _tenantRepository = tenantRepository;
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("MasterDb");
                var dbContext = new BackgroundDbContext(connectionString, _loggerFactory);

                while (!cancellationToken.IsCancellationRequested)
                {
                    var tenants = await dbContext.Tenants.ToListAsync(cancellationToken);

                    foreach (var tenant in tenants)
                    {
                        foreach (var user in tenant.Users)
                        {
                            var processes = await dbContext.Processes.Where(p => p.OwnerId == user.Id && p.TenantId == tenant.Id).ToListAsync(cancellationToken);
                            await HandleProcessesAsync(dbContext, user, processes, cancellationToken);
                        }
                    }

                    await Task.Delay(5000, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, $"{GetType().Name} occurred an error");
            }
        }

        //private Expression<Func<JobTenant, JobUser, Process, bool>> expression = (tenant, user, p) => p.OwnerId == user.Id && p.TenantId == tenant.Id;

        private async Task HandleProcessesAsync(BackgroundDbContext dbContext, JobUser user, List<Process> processes, CancellationToken cancellationToken)
        {
            var emailOptions = new EmailOptionRequest(user.Email, "", "");
            foreach (var process in processes)
            {
                dbContext.Entry(process).Reload();
                if (process.LastNotificationTime < DateHelper.Now.AddMinutes(-process.Period))
                {
                    process.LastNotificationTime = process.LastNotificationTime.AddMinutes(process.Period);
                    process.LastModifiedDate = DateHelper.Now;
                    process.Percent += CalculatePercent(process);

                    emailOptions.Subject = $"Notify at {process.LastNotificationTime.DateFullText()}";
                    emailOptions.Body = BuildEmailContent(user, process);

                    if (process.Percent >= 100)
                    {
                        process.Percent = 0;
                        process.Consecutiveness++;
                    }
                    _ = Task.Run(() =>
                    {
                        Logging.Information("sending...", true);
                        EmailHelper.SendMail(emailOptions);
                    });
                }
                else
                {
                    Logging.Information("chua dc dau soi a", true);
                }
            }
            await dbContext.CommitAsync();
        }

        private double CalculatePercent(Process process)
        {
            var minutes = (process.ToDate - process.FromDate).TotalMinutes;
            var percent = (process.Period * 1.0 / minutes) * 100;
            return Math.Round(percent, 2);
        }

        private string BuildEmailContent(JobUser user, Process process)
        {
            var content = "";
            var greeting = $"<h3 style='text-align: center; margin: 4px 0'>A new update about your process</h3> <div>Hi <strong>{user.FirstName}</strong>,</div><br/>";
            var endOfEmail = $@"<div style='color: #555555'>Thanks,</div> <div style='color: #555555'>The OpenVN Team</div>";
            var wrapper = "<div class='wrapper' style='border: 1px solid #e5e5e5; border-radius: 6px; min-width: 300px; max-width: 400px; max-height: 98%; padding: 12px 16px; margin: 24px auto; background-color: #fff;'>";
            if (process.Percent >= 100)
            {
                content = $"<div>Good job bro, you have completed <strong>{process.Consecutiveness + 1}</strong> progress cycle{(process.Consecutiveness + 1 > 1 ? "s" : "")}. <div>Try your best 😍</div></div><br/>";
            }
            else
            {
                content = $"<div>You have completed <strong>{process.Percent}%</strong> progress. <div>Try your best 👍</div></div><br/>";
            }

            return $"{wrapper}{greeting}{content}{endOfEmail}</div>";
        }
    }
}
