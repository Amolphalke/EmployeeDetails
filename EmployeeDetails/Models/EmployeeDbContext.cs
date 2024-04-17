using Microsoft.EntityFrameworkCore;

namespace EmployeeDetails.Models
{
    public class EmployeeDbContext:DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options):base(options) 
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<City> Cities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.City)
                .WithMany()
                .HasForeignKey(e => e.City_id); // Configure foreign key
        }
    }
}
