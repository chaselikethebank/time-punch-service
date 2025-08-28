using Microsoft.EntityFrameworkCore;
using TimePunchService.Models;

namespace TimePunchService.Data
{
    public class TimePunchDbContext : DbContext
    {
        public TimePunchDbContext(DbContextOptions<TimePunchDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<TimePunch> TimePunches { get; set; }
    }
}