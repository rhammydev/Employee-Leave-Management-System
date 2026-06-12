namespace EmployeeLeaveManagementSystem.Models.DTOs;

public class EmployeeResponseDto
{
    public int Id { get; set; }
    
    public string FullName { get; set; }
    
    public string Email { get; set; }
    
    public string Department { get; set; }

    public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    
    public ICollection<Leave> LeaveRequests { get; set; } = new List<Leave>();
}