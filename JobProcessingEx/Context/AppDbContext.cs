using JobProcessingEx.Models;
using Microsoft.EntityFrameworkCore;

namespace JobProcessingEx.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Job> Jobs { get; set; }
    }
}
