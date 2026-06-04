namespace EmployeeLeaveManagementSystem.Models;

public class LeaveRequest
{
    public int Id { get; set; }
    
    public int EmployeeId { get; set; }
    
    public LeaveType LeaveType { get; set; }
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly EndDate { get; set; }
    
    public string Reason { get; set; }
    
    public LeaveStatus  Status { get; set; }
    
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    
    public Employee Employee { get; set; }
    
    public string? RejectionReason { get; set; } 
}