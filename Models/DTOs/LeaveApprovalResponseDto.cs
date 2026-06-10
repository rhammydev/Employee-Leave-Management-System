namespace EmployeeLeaveManagementSystem.Models.DTOs;

public class LeaveApprovalResponseDto
{
    public int ApproverId { get; set; }
    
    public string ApproverName { get; set; }
    
    public string Action { get; set; }
    
    public string? Reason { get; set; }
    
    public DateTime DateActed { get; set; }
}