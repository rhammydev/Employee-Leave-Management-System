using EmployeeLeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Leave> LeaveRequests { get; set; }
    
    public DbSet<LeaveApproval> LeaveApprovals { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Leave>()
            .HasOne(lr => lr.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(lr => lr.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Leave>()
            .HasMany(lr => lr.Approvals)
            .WithOne(a=> a.LeaveRequest )
            .HasForeignKey(a => a.LeaveRequestId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<LeaveApproval>()
            .HasOne(lr => lr.Approver)
            .WithMany()
            .HasForeignKey(lr => lr.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);

      
    }
}