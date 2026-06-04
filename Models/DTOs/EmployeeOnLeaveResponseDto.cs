namespace EmployeeLeaveManagementSystem.Models.DTOs;

public class EmployeeOnLeaveResponseDto
{
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public LeaveType LeaveType { get; set; }
}