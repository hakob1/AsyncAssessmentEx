using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TransactionScopeEx.Context;
using TransactionScopeEx.Services;

class Program
{
    static async Task Main()
    {
        // Setup a service collection with EF and our services
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(options =>
        {
            //options
        });
        services.AddTransient<UserService>();
        services.AddTransient<PaymentService>();
        services.AddTransient<AccountSetupService>();

        var provider = services.BuildServiceProvider();

        var accountSetup = provider.GetRequiredService<AccountSetupService>();

        try
        {
            await accountSetup.SetUpNewUserAccountAsync("newuser@example.com", 100.00m);
            Console.WriteLine("User and initial payment created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Operation failed: {ex.Message}");
        }

        //Tasktodo
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        int userCount = await context.Users.CountAsync();
        int paymentCount = await context.Payments.CountAsync();
        Console.WriteLine($"Users in DB: {userCount}, Payments in DB: {paymentCount}");
    }
}
