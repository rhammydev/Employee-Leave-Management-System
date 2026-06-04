namespace EmployeeLeaveManagementSystem.Models.DTOs;

public class LeaveRequestResponseDto
{
    public int Id { get; set; }
    public string EmployeeName { get; set; }
    public string Department { get; set; }
    public LeaveType LeaveType { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Reason { get; set; }
    public LeaveStatus Status { get; set; }
    public DateTime DateCreated { get; set; }
    
    public string? RejectionReason { get; set; }
}