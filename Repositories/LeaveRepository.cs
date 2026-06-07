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
            throw new Exception("Employee already has a leave request overlapping this period.");

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
            throw new Exception("Only pending leave requests can be updated.");
        
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
            throw new Exception("Employee already has a leave request overlapping this period.");
        
        leaveExist.EmployeeId = leaveRequestDto.EmployeeId;
        leaveExist.StartDate = leaveRequestDto.StartDate;
        leaveExist.EndDate = leaveRequestDto.EndDate;
        leaveExist.Reason = leaveRequestDto.Reason;
        leaveExist.LeaveType = leaveRequestDto.LeaveType;
     
        
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

    public async Task<LeaveRequest> ApproveLeaveRequest(int id)
    {
        var leave = await _dbContext.LeaveRequests.FirstOrDefaultAsync(lr => lr.Id == id);
        if (leave == null)
        {
            throw new Exception("Leave Request Not Found");
        }

        if (leave.Status == LeaveStatus.Approved)
        {
            throw new Exception("Approved leave requests cannot be approved.");
        }

        if (leave.Status == LeaveStatus.Rejected)
        {
            throw new Exception("Rejected leave requests cannot be approved.");
        }

        leave.Status = LeaveStatus.Approved;
        await _dbContext.SaveChangesAsync();
        return leave;
    }

    public async Task<LeaveRequest> RejectLeaveRequest(int id, LeaveRejectDto leaveRejectDto)
    {
        var leave = await _dbContext.LeaveRequests.FirstOrDefaultAsync(lr => lr.Id == id);
        if (leave == null)
        {
            throw new Exception("Leave Request Not Found");
        }

        if (leave.Status == LeaveStatus.Rejected)
        {
            throw new Exception("Rejected leave requests cannot be rejected");
        }

        if (leave.Status == LeaveStatus.Approved)
        {
            throw new Exception("Approved leave requests cannot be rejected");
        }

        if (leaveRejectDto.RejectionReason == string.Empty || leaveRejectDto.RejectionReason.Length < 5)
        {
            throw new Exception("Please provide a meaningful rejection reason");
        }

        leave.RejectionReason = leaveRejectDto.RejectionReason;
        leave.Status = LeaveStatus.Rejected;
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
               Pending = s.Count(lr => lr.Status == LeaveStatus.Pending),
               Approved = s.Count(lr => lr.Status == LeaveStatus.Approved),
               Rejected = s.Count(lr => lr.Status == LeaveStatus.Rejected)
           }).FirstOrDefaultAsync();
       
       return stats;
    }

    public async Task<IEnumerable<EmployeeOnLeaveResponseDto>> GetEmployeesOnLeave()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var employees = await _dbContext.Employees
            .Include(e => e.LeaveRequests)
            .Where(e => e.LeaveRequests.Any(lr => 
            lr.Status == LeaveStatus.Approved && 
            lr.StartDate <= today && 
            lr.EndDate >= today))
            .SelectMany(e => e.LeaveRequests
                .Where(lr => lr.Status == LeaveStatus.Approved && 
                         lr.StartDate <= today && 
                         lr.EndDate >= today)
                .Select(lr => new EmployeeOnLeaveResponseDto
                {
                Id = e.Id,
                EmployeeName = e.FullName,
                Department = e.Department,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                LeaveType =  lr.LeaveType
                }))
            .ToListAsync();
        
        return employees;
    }

    public async Task<IEnumerable<LeaveRequestResponseDto>> GetLeaveByStatus(string status)
    {
        if (!Enum.TryParse<LeaveStatus>(status, ignoreCase: true, out var leaveStatus))
            throw new Exception($"Invalid status '{status}'. Valid values are: Pending, Approved, Rejected.");

        return await _dbContext.LeaveRequests
            .Include(lr => lr.Employee)
            .Where(lr => lr.Status == leaveStatus)
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