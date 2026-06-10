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
    
    public Task<LeaveRequestResponseDto> ApproveLeaveRequest(int id, ApproveLeaveRequestDto approveLeaveRequestDto);
    
    public Task<LeaveRequestResponseDto> RejectLeaveRequest(int id, RejectLeaveRequestDto rejectLeaveRequestDto);
    
    public Task<LeaveStatisticsResponseDto> GetLeavesStatsByDepartment(string department);
    
    public Task<IEnumerable<LeaveRequestResponseDto>> GetLeaveByStatus(string status);
}