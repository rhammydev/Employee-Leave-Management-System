using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.DTOs;

namespace EmployeeLeaveManagementSystem.Repositories;

public interface ILeaveRepository
{
    public Task<IEnumerable<LeaveRequestResponseDto>> GetAllLeaveRequests();
    
    public Task<LeaveRequest> GetLeaveRequestById(int id);
    
    public Task<LeaveRequest> SubmitLeaveRequest(LeaveRequestDto leaveRequestDto);
    
    public Task<LeaveRequest> UpdateLeaveRequest(int id, LeaveRequestDto leaveRequestDto);
    
    public Task<bool> DeleteLeaveRequest(int id);
    
    public Task<LeaveRequest> ApproveLeaveRequest(int id);
    
    public Task<LeaveRequest> RejectLeaveRequest(int id);
    
    public Task<LeaveStatisticsResponseDto> GetLeavesStatsByDepartment(string department);

    public Task<IEnumerable<Employee>> GetEmployeesOnLeave();
    
    public Task<IEnumerable<LeaveRequestResponseDto>> GetLeaveByStatus(string status);
}