namespace EmployeeLeaveManagementSystem.Models.DTOs;

public class UpdateEmployeeRequestDto
{
    public string FullName { get; set; }
    
    public string Email { get; set; }
    
    public string Department { get; set; }
}