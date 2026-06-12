namespace EmployeeLeaveManagementSystem.Models.DTOs;

public class ApproveLeaveRequestDto
{
    public int ApproverId { get; set; }
    
    public string? Reason { get; set; }
}