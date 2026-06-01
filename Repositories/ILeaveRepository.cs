using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.DTOs;

namespace EmployeeLeaveManagementSystem.Repositories;

public interface ILeaveRepository
{
    public Task<IEnumerable<LeaveRequest>> GetAllLeaveRequests();
    
    public Task<LeaveRequest> GetLeaveRequestById(int id);
    
    public Task<LeaveRequest> SubmitLeaveRequest(LeaveRequestDto leaveRequestDto);
    
    public Task<LeaveRequest> UpdateLeaveRequest(int id, LeaveRequestDto leaveRequestDto);
    
    public Task<bool> DeleteLeaveRequest(int id);
}