using EmployeeLeaveManagementSystem.Constants;
using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.Exceptions;
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
            .Include(lr => lr.Approvals)
            .ThenInclude(a => a.Approver)
            .Select(lr => new LeaveRequestResponseDto
            {
                Id = lr.Id,
                EmployeeName = lr.Employee.FullName,
                EmployeeId = lr.Employee.Id,
                Department = lr.Employee.Department,
                LeaveType = lr.LeaveType,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Reason = lr.Reason,
                Status = lr.Status,
                DateCreated = lr.DateCreated,
                RejectionReason = lr.RejectionReason,
                Approvals = lr.Approvals.Select(a => new LeaveApprovalResponseDto
                {
                    ApproverId = a.ApproverId,
                    ApproverName = a.Approver.FullName,
                    Action = a.Action,
                    Reason = a.Reason,
                    DateActed = a.DateActed
                }).ToList()
            })
            .ToListAsync();

        return leaveRequests;
    }

    public async Task<LeaveRequestResponseDto> GetLeaveRequestById(int id)
    {
        var leaveRequest = await _dbContext.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.Approvals)
            .ThenInclude(a => a.Approver)
            .Select(lr => new LeaveRequestResponseDto
            {
                Id = lr.Id,
                EmployeeName = lr.Employee.FullName,
                EmployeeId = lr.Employee.Id,
                Department = lr.Employee.Department,
                LeaveType = lr.LeaveType,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Reason = lr.Reason,
                Status = lr.Status,
                DateCreated = lr.DateCreated,
                RejectionReason = lr.RejectionReason,
                Approvals = lr.Approvals.Select(a => new LeaveApprovalResponseDto
                {
                    ApproverId = a.ApproverId,
                    ApproverName = a.Approver.FullName,
                    Action = a.Action,
                    Reason = a.Reason,
                    DateActed = a.DateActed
                }).ToList()
            })
            .FirstOrDefaultAsync(x => x.Id == id);

        return leaveRequest ?? throw new NotFoundException("Leave Request Not Found");
    }

    public async Task<Leave> SubmitLeaveRequest(SubmitLeaveRequestDto submitLeaveRequestDto)
    {
        var employeeExist = await _dbContext.Employees.AnyAsync(e => e.Id == submitLeaveRequestDto.EmployeeId);
        if (!employeeExist)
        {
            throw new NotFoundException("Employee not found");
        }
        
        if(submitLeaveRequestDto.StartDate > submitLeaveRequestDto.EndDate)
        {
            throw new BadRequestException("Start Date cannot be later than End Date.");
        }
        
        if (submitLeaveRequestDto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new BadRequestException("Start Date cannot be in the past.");
       
        bool hasOverlap = await _dbContext.LeaveRequests
            .AnyAsync(lr =>
                lr.EmployeeId == submitLeaveRequestDto.EmployeeId &&
                lr.Status != LeaveConstants.Rejected &&   
                lr.StartDate <= submitLeaveRequestDto.EndDate &&
                lr.EndDate >= submitLeaveRequestDto.StartDate
            );

        if (hasOverlap)
            throw new ConflictException("Employee already has a leave request overlapping this period.");

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
            throw new NotFoundException("Leave Request Not Found");
        }
        
        var employeeExist = await _dbContext.Employees.AnyAsync(e => e.Id == submitLeaveRequestDto.EmployeeId);
        if (!employeeExist)
        {
            throw new NotFoundException("Employee not found");
        }
        
        if (leaveExist.Status != LeaveConstants.Pending)
            throw new ConflictException("Only pending leave requests can be updated.");
        
        if(submitLeaveRequestDto.StartDate > submitLeaveRequestDto.EndDate)
        {
            throw new BadRequestException("Start Date cannot be later than End Date.");
        }
        
        if (submitLeaveRequestDto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new BadRequestException("Start Date cannot be in the past.");
        
        bool hasOverlap = await _dbContext.LeaveRequests
            .AnyAsync(lr =>
                lr.Id != id &&  
                lr.EmployeeId == submitLeaveRequestDto.EmployeeId &&
                lr.Status != LeaveConstants.Rejected &&
                lr.StartDate <= submitLeaveRequestDto.EndDate &&
                lr.EndDate >= submitLeaveRequestDto.StartDate
            );

        if (hasOverlap)
            throw new ConflictException("Employee already has a leave request overlapping this period.");
        
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
            throw new NotFoundException("Leave Request Not Found");
        }
        
        if (leave.Status == LeaveConstants.Approved)
            throw new ConflictException("Approved leave requests cannot be deleted.");
        
        _dbContext.LeaveRequests.Remove(leave);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<LeaveRequestResponseDto> ApproveLeaveRequest(int id, LeaveActionRequestDto leaveActionRequestDto)
    {
        var leave = await _dbContext.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.Approvals)
            .FirstOrDefaultAsync(lr => lr.Id == id);
        
        
        if (leave == null)
        {
            throw new NotFoundException("Leave Request Not Found");
        }

        if (leave.Status == LeaveConstants.Approved)
        {
            throw new ConflictException("Approved leave requests cannot be approved.");
        }

        if (leave.Status == LeaveConstants.Rejected)
        {
            throw new BadRequestException("Rejected leave requests cannot be approved.");
        }

        if (leaveActionRequestDto.ApproverId == leave.EmployeeId)
        {
            throw new BadRequestException("You are not allowed to approve your own request.");
        }
        
        var alreadyActed = leave.Approvals.Any(a 
            => a.ApproverId == leaveActionRequestDto.ApproverId);
        
        if (alreadyActed)
        {
            throw new ConflictException(
                "You have already taken an action on this leave request.");
        }
        
        var approvalCount =
            leave.Approvals.Count(a => a.Action == LeaveConstants.Approved);

        if (approvalCount == 0)
        {
            leave.Status = LeaveConstants.Processing;
        }
        else if (approvalCount == 1)
        {
            leave.Status = LeaveConstants.Approved;
        }

        var leaveApproval = new LeaveApproval()
        {
            LeaveRequestId = leave.Id,
            ApproverId = leaveActionRequestDto.ApproverId,
            Action = LeaveConstants.Approved,
            Reason = leaveActionRequestDto.Reason,
            DateActed = DateTime.UtcNow
        };
        
        await _dbContext.LeaveApprovals.AddAsync(leaveApproval);
        leave.Approvals.Add(leaveApproval);
        await _dbContext.SaveChangesAsync();
        return await GetLeaveRequestById(id);
    }

    public async Task<LeaveRequestResponseDto> RejectLeaveRequest(int id, LeaveActionRequestDto leaveActionRequestDto)
    {
        var leave = await _dbContext.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.Approvals)
            .FirstOrDefaultAsync(lr => lr.Id == id);
        
        if (leave == null)
        {
            throw new NotFoundException("Leave Request Not Found");
        }

        if (leave.Status == LeaveConstants.Rejected)
        {
            throw new ConflictException("Rejected leave requests cannot be rejected");
        }

        if (leave.Status == LeaveConstants.Approved)
        {
            throw new ConflictException("Approved leave requests cannot be rejected");
        }

        if (leaveActionRequestDto.ApproverId == leave.EmployeeId)
        {
            throw new BadRequestException("You are not allowed to reject your own request.");
        }
        
        var alreadyActed = leave.Approvals.Any(a 
            => a.ApproverId == leaveActionRequestDto.ApproverId);
        
        if (alreadyActed)
        {
            throw new ConflictException(
                "You have already taken an action on this leave request.");
        }
        
        leave.Status = LeaveConstants.Rejected;
        leave.RejectionReason = leaveActionRequestDto.Reason;

        var leaveApproval = new LeaveApproval()
        {
            LeaveRequestId = leave.Id,
            ApproverId = leaveActionRequestDto.ApproverId,
            Action = LeaveConstants.Rejected,
            Reason = leaveActionRequestDto.Reason,
            DateActed = DateTime.UtcNow
        };
        
        await _dbContext.LeaveApprovals.AddAsync(leaveApproval);
        await _dbContext.SaveChangesAsync();
        return await GetLeaveRequestById(id);
    }

    public async Task<LeaveStatisticsResponseDto> GetLeavesStatsByDepartment(string department)
    {
       var departmentExists = await _dbContext.Employees.AnyAsync(e => e.Department.ToLower() == department.ToLower());
       if (!departmentExists)
       {
           throw new NotFoundException($"No employee found in {department} deparment");
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
             throw new BadRequestException($"Action must be one of {string.Join(", ", LeaveConstants.JoinedStatus)}");
         }
       
       return await _dbContext.LeaveRequests
           .Include(lr => lr.Employee)
           .Where(lr => lr.Status == status)
           .Select(lr => new LeaveRequestResponseDto
           {
               Id = lr.Id,
               EmployeeName = lr.Employee.FullName,
               EmployeeId = lr.Employee.Id,
               Department = lr.Employee.Department,
               LeaveType = lr.LeaveType,
               StartDate = lr.StartDate,
               EndDate = lr.EndDate,
               Reason = lr.Reason,
               Status = lr.Status,
               DateCreated = lr.DateCreated,
               RejectionReason = lr.RejectionReason,
               Approvals = lr.Approvals.Select(a => new LeaveApprovalResponseDto
               {
                   ApproverId = a.ApproverId,
                   ApproverName = a.Approver.FullName,
                   Action = a.Action,
                   Reason = a.Reason,
                   DateActed = a.DateActed
               }).ToList()
           })
           .ToListAsync();
    } 
}