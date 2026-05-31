namespace EmployeeLeaveManagementSystem.Models;

public class LeaveRequest
{
    public int LeaveId { get; set; }
    
    public int EmployeeId { get; set; }
    
    public string LeaveTypeId { get; set; }
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly EndDate { get; set; }
    
    public string Reason { get; set; }
    
    public LeaveStatus  Status { get; set; }
    
    public DateTime DateCreated { get; set; }
}