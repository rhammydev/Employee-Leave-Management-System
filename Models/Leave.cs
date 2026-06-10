namespace EmployeeLeaveManagementSystem.Models;

public class Leave
{
    public int Id { get; set; }
    
    public int EmployeeId { get; set; }
    
    public string LeaveType { get; set; }
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly EndDate { get; set; }
    
    public string Reason { get; set; }
    
    public string  Status { get; set; }
    
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    
    public string? RejectionReason { get; set; } 
    
    public Employee Employee { get; set; }
    
    public ICollection<LeaveApproval> Approvals { get; set; }
        = new List<LeaveApproval>();
}