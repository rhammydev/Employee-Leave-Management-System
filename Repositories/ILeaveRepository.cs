using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.DTOs;

namespace EmployeeLeaveManagementSystem.Repositories;

public interface ILeaveRepository
{
    public Task<IEnumerable<LeaveRequestResponseDto>> GetAllLeaveRequests();
    
    public Task<LeaveRequestResponseDto> GetLeaveRequestById(int id);
    
    public Task<Leave> SubmitLeaveRequest(SubmitLeaveRequestDto submitLeaveRequestDto);
    
    public Task<Leave> UpdateLeaveRequest(int id, SubmitLeaveRequestDto submitLeaveRequestDto);
    
    public Task<bool> DeleteLeaveRequest(int id);
    
    public Task<Leave> ApproveLeaveRequest(int id, LeaveActionRequestDto leaveActionRequestDto);
    
    public Task<Leave> RejectLeaveRequest(int id, LeaveActionRequestDto leaveActionRequestDto);
    
    public Task<LeaveStatisticsResponseDto> GetLeavesStatsByDepartment(string department);
    
    public Task<IEnumerable<LeaveRequestResponseDto>> GetLeaveByStatus(string status);
}