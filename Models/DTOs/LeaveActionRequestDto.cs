namespace EmployeeLeaveManagementSystem.Models.DTOs;

public class LeaveActionRequestDto
{
    public int LeaveRequestId { get; set; }
    
    public int ApproverId { get; set; }
    
    public string Action { get; set; }
    
    public string? Reason { get; set; }
}