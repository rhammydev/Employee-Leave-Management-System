namespace EmployeeLeaveManagementSystem.Models.DTOs;

public class SubmitLeaveRequestDto
{
    public int EmployeeId { get; set; }
    
    public string LeaveType { get; set; }
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly EndDate { get; set; }
    
    public string Reason { get; set; }
}