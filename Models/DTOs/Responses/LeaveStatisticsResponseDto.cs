namespace EmployeeLeaveManagementSystem.Models.DTOs;

public class LeaveStatisticsResponseDto
{
    public string Department { get; set; }
    public int TotalRequests { get; set; }
    public int Pending { get; set; }
    
    public int Processing { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
}