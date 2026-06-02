using EmployeeLeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Employee> Employees { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }

    // ERD
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LeaveRequest>()
            .HasOne(lr => lr.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(lr => lr.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LeaveRequest>()
            .Property(lr => lr.LeaveType)
            .HasConversion<string>();
        
        modelBuilder.Entity<LeaveRequest>()
            .Property(lr => lr.Status)
            .HasConversion<string>();
    }
}