using EmployeeManagement.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
    .HasOne(e => e.Department)
    .WithMany(d => d.Employees)
    .HasForeignKey(e => e.DepartmentId)
    .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "IT", Location = "Block A" },
            new Department { Id = 2, Name = "HR", Location = "Block B" }
        );
    }
}