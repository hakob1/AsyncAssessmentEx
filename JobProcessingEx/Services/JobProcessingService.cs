using JobProcessingEx.Context;
using JobProcessingEx.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Transactions;

namespace JobProcessingEx.Services
{
    public class JobProcessingService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public JobProcessingService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _ = ProcessPendingJobsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in job processing: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        //Tasktodo
        private async Task ProcessPendingJobsAsync(CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var pendingJobs = await context.Jobs
                .Where(j => !j.Processed)
                .OrderBy(j => j.CreatedAt)
                .Take(10)
                .ToListAsync(token);

            if (pendingJobs.Count == 0)
            {
                Console.WriteLine("No pending jobs found.");
                return;
            }

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            foreach (var job in pendingJobs)
            {
                token.ThrowIfCancellationRequested();
                await ProcessJobAsync(job, token);
                job.Processed = true;
            }

            await context.SaveChangesAsync(token);

            transactionScope.Complete();

            Console.WriteLine($"{pendingJobs.Count} jobs processed and committed.");
        }

        private async Task ProcessJobAsync(Job job, CancellationToken token)
        {
            await Task.Delay(200, token);
        }
    }
}
