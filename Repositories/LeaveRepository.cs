using EmployeeLeaveManagementSystem.Constants;
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
    
    public async Task<IEnumerable<LeaveRequestResponseDto>> GetAllLeaveRequests()
    {
        var leaveRequests = await _dbContext.LeaveRequests
            .Include(lr => lr.Employee)
            .Select(lr => new LeaveRequestResponseDto
            {
                Id = lr.Id,
                EmployeeName = lr.Employee.FullName,
                Department = lr.Employee.Department,
                LeaveType = lr.LeaveType,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Reason = lr.Reason,
                Status = lr.Status,
                DateCreated = lr.DateCreated,
                RejectionReason = lr.RejectionReason,
            })
            .ToListAsync();

        if (leaveRequests.Count == 0)
        {
            throw new Exception("Leave Request Not Found");
        }

        return leaveRequests;
    }

    public async Task<LeaveRequestResponseDto> GetLeaveRequestById(int id)
    {
        var leaveRequest = await _dbContext.LeaveRequests
            .Include(lr => lr.Employee)
            .Select(lr => new LeaveRequestResponseDto
            {
                Id = lr.Id,
                EmployeeName = lr.Employee.FullName,
                Department = lr.Employee.Department,
                LeaveType = lr.LeaveType,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Reason = lr.Reason,
                Status = lr.Status,
                DateCreated = lr.DateCreated,
                RejectionReason = lr.RejectionReason,
            })
            .FirstOrDefaultAsync(x => x.Id == id);

        return leaveRequest ?? throw new Exception("Leave Request Not Found");
    }

    public async Task<Leave> SubmitLeaveRequest(SubmitLeaveRequestDto submitLeaveRequestDto)
    {
        var employeeExist = await _dbContext.Employees.AnyAsync(e => e.Id == submitLeaveRequestDto.EmployeeId);
        if (!employeeExist)
        {
            throw new Exception("Employee not found");
        }
        
        if(submitLeaveRequestDto.StartDate > submitLeaveRequestDto.EndDate)
        {
            throw new Exception("Start Date cannot be later than End Date.");
        }
        
        if (submitLeaveRequestDto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new Exception("Start Date cannot be in the past.");
       
        bool hasOverlap = await _dbContext.LeaveRequests
            .AnyAsync(lr =>
                lr.EmployeeId == submitLeaveRequestDto.EmployeeId &&
                lr.Status != LeaveConstants.Rejected &&   
                lr.StartDate <= submitLeaveRequestDto.EndDate &&
                lr.EndDate >= submitLeaveRequestDto.StartDate
            );

        if (hasOverlap)
            throw new Exception("Employee already has a leave request overlapping this period.");

        var leaveRequest = new Leave()
        {
            EmployeeId = submitLeaveRequestDto.EmployeeId,
            StartDate = submitLeaveRequestDto.StartDate,
            EndDate = submitLeaveRequestDto.EndDate,
            LeaveType = submitLeaveRequestDto.LeaveType,
            Reason = submitLeaveRequestDto.Reason,
            Status = LeaveConstants.Pending
        };
        
        await _dbContext.LeaveRequests.AddAsync(leaveRequest);
        await _dbContext.SaveChangesAsync();
        return leaveRequest;


    }

    public async Task<Leave> UpdateLeaveRequest(int id, SubmitLeaveRequestDto submitLeaveRequestDto)
    {
        var leaveExist = await _dbContext.LeaveRequests.FirstOrDefaultAsync(lr => lr.Id == id);
        if (leaveExist  == null)
        {
            throw new Exception("Leave Request Not Found");
        }
        
        var employeeExist = await _dbContext.Employees.AnyAsync(e => e.Id == submitLeaveRequestDto.EmployeeId);
        if (!employeeExist)
        {
            throw new Exception("Employee not found");
        }
        
        if (leaveExist.Status != LeaveConstants.Pending)
            throw new Exception("Only pending leave requests can be updated.");
        
        if(submitLeaveRequestDto.StartDate > submitLeaveRequestDto.EndDate)
        {
            throw new Exception("Start Date cannot be later than End Date.");
        }
        
        if (submitLeaveRequestDto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new Exception("Start Date cannot be in the past.");
        
        bool hasOverlap = await _dbContext.LeaveRequests
            .AnyAsync(lr =>
                lr.Id != id &&  
                lr.EmployeeId == submitLeaveRequestDto.EmployeeId &&
                lr.Status != LeaveConstants.Rejected &&
                lr.StartDate <= submitLeaveRequestDto.EndDate &&
                lr.EndDate >= submitLeaveRequestDto.StartDate
            );

        if (hasOverlap)
            throw new Exception("Employee already has a leave request overlapping this period.");
        
        leaveExist.EmployeeId = submitLeaveRequestDto.EmployeeId;
        leaveExist.StartDate = submitLeaveRequestDto.StartDate;
        leaveExist.EndDate = submitLeaveRequestDto.EndDate;
        leaveExist.Reason = submitLeaveRequestDto.Reason;
        leaveExist.LeaveType = submitLeaveRequestDto.LeaveType;
     
        
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
        
        if (leave.Status == LeaveConstants.Approved)
            throw new InvalidOperationException("Approved leave requests cannot be deleted.");
        
        _dbContext.LeaveRequests.Remove(leave);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Leave> ApproveLeaveRequest(int id, LeaveActionRequestDto leaveActionRequestDto)
    {
        var leave = await _dbContext.LeaveRequests.FirstOrDefaultAsync(lr => lr.Id == id);
        if (leave == null)
        {
            throw new Exception("Leave Request Not Found");
        }

        if (leave.Status == LeaveConstants.Approved)
        {
            throw new Exception("Approved leave requests cannot be approved.");
        }

        if (leave.Status == LeaveConstants.Rejected)
        {
            throw new Exception("Rejected leave requests cannot be approved.");
        }

        if (leaveActionRequestDto.ApproverId == leave.EmployeeId)
        {
            throw new Exception("You are not allowed to approve your own request.");
        }

        var leaveApproval = new LeaveApproval()
        {
            LeaveRequestId = leave.Id,
            ApproverId = leaveActionRequestDto.ApproverId,
            Action = leave.Approvals.Count == 1 ?  LeaveConstants.Processing : LeaveConstants.Approved,
            Reason = leaveActionRequestDto.Reason,
        };
        
        await _dbContext.LeaveApprovals.AddAsync(leaveApproval);
        await _dbContext.SaveChangesAsync();
        return leave;
    }

    public async Task<Leave> RejectLeaveRequest(int id, LeaveActionRequestDto leaveActionRequestDto)
    {
        var leave = await _dbContext.LeaveRequests.FirstOrDefaultAsync(lr => lr.Id == id);
        if (leave == null)
        {
            throw new Exception("Leave Request Not Found");
        }

        if (leave.Status == LeaveConstants.Rejected)
        {
            throw new Exception("Rejected leave requests cannot be rejected");
        }

        if (leave.Status == LeaveConstants.Approved)
        {
            throw new Exception("Approved leave requests cannot be rejected");
        }

        if (leaveActionRequestDto.Reason == string.Empty || leaveActionRequestDto.Reason.Length < 5)
        {
            throw new Exception("Please provide a meaningful rejection reason");
        }

        var leaveApproval = new LeaveApproval()
        {
            LeaveRequestId = leave.Id,
            ApproverId = leaveActionRequestDto.ApproverId,
            Action = LeaveConstants.Rejected,
            Reason = leaveActionRequestDto.Reason,
        };
        
        await _dbContext.LeaveApprovals.AddAsync(leaveApproval);
        await _dbContext.SaveChangesAsync();
        return leave;
    }

    public async Task<LeaveStatisticsResponseDto> GetLeavesStatsByDepartment(string department)
    {
       var departmentExists = await _dbContext.Employees.AnyAsync(e => e.Department.ToLower() == department.ToLower());
       if (!departmentExists)
       {
           throw new Exception($"No employee found in {department} deparment");
       }

       var stats = await _dbContext.LeaveRequests
           .Include(lr => lr.Employee)
           .Where(lr => lr.Employee.Department.ToLower() == department.ToLower())
           .GroupBy(lr => lr.Employee.Department)
           .Select(s => new LeaveStatisticsResponseDto
           {
               Department = s.Key,
               TotalRequests = s.Count(),
               Pending = s.Count(lr => lr.Status == LeaveConstants.Pending),
               Processing = s.Count(lr => lr.Status == LeaveConstants.Processing),
               Approved = s.Count(lr => lr.Status == LeaveConstants.Approved),
               Rejected = s.Count(lr => lr.Status == LeaveConstants.Rejected)
           }).FirstOrDefaultAsync();
       
       return stats;
    }
    
    public async Task<IEnumerable<LeaveRequestResponseDto>> GetLeaveByStatus(string status)
    {
       if (!LeaveConstants.ValidStatus.Contains(status.ToUpper()))
         {
             throw new Exception($"Action must be one of {string.Join(", ", LeaveConstants.JoinedStatus)}");
         }
       
       return await _dbContext.LeaveRequests
           .Include(lr => lr.Employee)
           .Where(lr => lr.Status == status)
           .Select(lr => new LeaveRequestResponseDto
           {
               Id = lr.Id,
               EmployeeName = lr.Employee.FullName,
               Department = lr.Employee.Department,
               LeaveType = lr.LeaveType,
               StartDate = lr.StartDate,
               EndDate = lr.EndDate,
               Reason = lr.Reason,
               Status = lr.Status,
               DateCreated = lr.DateCreated,
               RejectionReason = lr.RejectionReason,
           })
           .ToListAsync();


    } 
}