using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Repositories;

public class LeaveRepository: ILeaveRepository
{
    private readonly ApplicationDbContext _dbContext;
    public LeaveRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<LeaveRequest>> GetAllLeaveRequests()
    {
        return await _dbContext.LeaveRequests.ToListAsync();
    }

    public async Task<LeaveRequest> GetLeaveRequestById(int id)
    {
        var leaveRequest = await _dbContext.LeaveRequests.FirstOrDefaultAsync(x => x.Id == id);
        return leaveRequest ?? throw new Exception("Leave Request Not Found");
    }

    public async Task<LeaveRequest> SubmitLeaveRequest(LeaveRequestDto leaveRequestDto)
    {
        // employee must exist
        var employeeExist = await _dbContext.Employees.AnyAsync(e => e.Id == leaveRequestDto.EmployeeId);
        if (!employeeExist)
        {
            throw new Exception("Employee not found");
        }
        
        // start date can't be later than end date
        if(leaveRequestDto.StartDate > leaveRequestDto.EndDate)
        {
            throw new Exception("Start Date cannot be later than End Date.");
        }
        
        if (leaveRequestDto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new Exception("Start Date cannot be in the past.");
        
        // Check for overlapping requests for the same employee
        bool hasOverlap = await _dbContext.LeaveRequests
            .AnyAsync(lr =>
                lr.EmployeeId == leaveRequestDto.EmployeeId &&
                lr.Status != LeaveStatus.Rejected &&   // ignore rejected ones
                lr.StartDate <= leaveRequestDto.EndDate &&
                lr.EndDate >= leaveRequestDto.StartDate
            );

        if (hasOverlap)
            throw new InvalidOperationException("Employee already has a leave request overlapping this period.");


        var leaveRequest = new LeaveRequest()
        {
            EmployeeId = leaveRequestDto.EmployeeId,
            StartDate = leaveRequestDto.StartDate,
            EndDate = leaveRequestDto.EndDate,
            LeaveType = leaveRequestDto.LeaveType,
            Reason = leaveRequestDto.Reason,
            Status = LeaveStatus.Pending
        };
        
        await _dbContext.LeaveRequests.AddAsync(leaveRequest);
        await _dbContext.SaveChangesAsync();
        return leaveRequest;


    }

    public async Task<LeaveRequest> UpdateLeaveRequest(int id, LeaveRequestDto leaveRequestDto)
    {
        var leaveExist = await _dbContext.LeaveRequests.FirstOrDefaultAsync(lr => lr.Id == id);
        if (leaveExist  == null)
        {
            throw new Exception("Leave Request Not Found");
        }
        
        var employeeExist = await _dbContext.Employees.AnyAsync(e => e.Id == leaveRequestDto.EmployeeId);
        if (!employeeExist)
        {
            throw new Exception("Employee not found");
        }
        
        if (leaveExist.Status != LeaveStatus.Pending)
            throw new InvalidOperationException("Only pending leave requests can be updated.");
        
        if(leaveRequestDto.StartDate > leaveRequestDto.EndDate)
        {
            throw new Exception("Start Date cannot be later than End Date.");
        }
        
        if (leaveRequestDto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new Exception("Start Date cannot be in the past.");
        
        bool hasOverlap = await _dbContext.LeaveRequests
            .AnyAsync(lr =>
                lr.Id != id &&   // exclude the current request being updated
                lr.EmployeeId == leaveRequestDto.EmployeeId &&
                lr.Status != LeaveStatus.Rejected &&
                lr.StartDate <= leaveRequestDto.EndDate &&
                lr.EndDate >= leaveRequestDto.StartDate
            );

        if (hasOverlap)
            throw new InvalidOperationException("Employee already has a leave request overlapping this period.");
        
        leaveExist.EmployeeId = leaveRequestDto.EmployeeId;
        leaveExist.StartDate = leaveRequestDto.StartDate;
        leaveExist.EndDate = leaveRequestDto.EndDate;
        leaveExist.Reason = leaveRequestDto.Reason;
        leaveExist.LeaveType = leaveRequestDto.LeaveType;
        leaveExist.Status = leaveRequestDto.Status;
        
        await _dbContext.SaveChangesAsync();
        return leaveExist;
        
    }

    public async Task<bool> DeleteLeaveRequest(int id)
    {
        var leave = await _dbContext.LeaveRequests.FirstOrDefaultAsync(lr => lr.Id == id);
        if (leave == null)
        {
            throw new Exception("Leave Request Not Found");
        }
        
        if (leave.Status == LeaveStatus.Approved)
            throw new InvalidOperationException("Approved leave requests cannot be deleted.");
        
        _dbContext.LeaveRequests.Remove(leave);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}