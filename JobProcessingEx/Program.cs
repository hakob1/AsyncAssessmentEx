using JobProcessingEx.Context;
using JobProcessingEx.Models;
using JobProcessingEx.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

static async Task Main()
{
    var services = new ServiceCollection();
    services.AddDbContext<AppDbContext>(options => {
        //options
    });
    services.AddHostedService<JobProcessingService>();

    var provider = services.BuildServiceProvider();

    // Seed some jobs
    using (var scope = provider.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (!context.Jobs.Any())
        {
            context.Jobs.Add(new Job { Data = "Job1", CreatedAt = DateTime.UtcNow });
            context.Jobs.Add(new Job { Data = "Job2", CreatedAt = DateTime.UtcNow.AddSeconds(-10) });
            await context.SaveChangesAsync();
        }
    }

    var host = new HostBuilder()
        .ConfigureServices(s => s.AddSingleton(provider))
        .UseConsoleLifetime()
        .Build();

    await host.StartAsync();
    Console.WriteLine("Hosted service started. Press Ctrl+C to exit.");

    await host.WaitForShutdownAsync();
}