namespace EmployeeLeaveManagementSystem.Models.DTOs;

public class LeaveRequestDto
{
    public int EmployeeId { get; set; }
    
    public string LeaveTypeId { get; set; }
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly EndDate { get; set; }
    
    public string Reason { get; set; }
    
    public LeaveStatus  Status { get; set; }
}