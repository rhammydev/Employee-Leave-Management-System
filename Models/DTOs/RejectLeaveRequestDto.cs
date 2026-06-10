namespace EmployeeLeaveManagementSystem.Models.DTOs;

public class RejectLeaveRequestDto
{
    public int ApproverId { get; set; }
    
    public string Reason { get; set; }
}