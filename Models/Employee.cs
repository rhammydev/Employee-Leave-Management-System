namespace EmployeeLeaveManagementSystem.Models;

public class Employee
{
    public int EmployeeId { get; set; }
    
    public string FullName { get; set; }
    
    public string Email { get; set; }
    
    public string Department { get; set; }

    public DateTime DateJoined { get; set; } = DateTime.UtcNow;
}