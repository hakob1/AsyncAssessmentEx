using Microsoft.EntityFrameworkCore;
using TransactionScopeEx.Models;

namespace TransactionScopeEx.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }

}
