using EmployeeLeaveManagementSystem.Constants;
using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.Exceptions;
using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _dbContext;
    public EmployeeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<EmployeeResponseDto>> GetAllEmployees()
    {
        var employees = await _dbContext.Employees
            .Include(e => e.LeaveRequests)
            .Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Email = e.Email,
                Department = e.Department,
                DateJoined =  e.DateJoined,
                LeaveRequests = e.LeaveRequests
            })
            .ToListAsync();
        
        return employees ?? throw new Exception("Employee not found");
    }

    public async Task<EmployeeResponseDto> GetEmployeeById(int employeeId)
    {
        var employee = await _dbContext.Employees
            .Include(e => e.LeaveRequests)
            .Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Email = e.Email,
                Department = e.Department,
                DateJoined =  e.DateJoined,
                LeaveRequests = e.LeaveRequests
            })
            .FirstOrDefaultAsync(e => e.Id == employeeId);
        return employee ?? throw new NotFoundException("Employee not found");
    }

    public async Task<Employee> CreateEmployee(CreateEmployeeRequestDto createEmployeeRequestDto)
    {
        var employeeExist = await _dbContext.Employees.AnyAsync(e => e.Email.ToLower() == createEmployeeRequestDto.Email.ToLower());
        if (employeeExist)
        {
            throw new ConflictException("Employee already exists");
        }

        var employee = new Employee()
        {
            FullName = createEmployeeRequestDto.FullName,
            Email = createEmployeeRequestDto.Email,
            Department = createEmployeeRequestDto.Department,
        };
        
        await _dbContext.Employees.AddAsync(employee);
        await _dbContext.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateEmployee(int id, UpdateEmployeeRequestDto updateEmployeeRequestDto)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);
        
        var employeeExist = await _dbContext.Employees.AnyAsync(e => 
            e.Email.ToLower() == updateEmployeeRequestDto.Email.ToLower()
            && e.Id != id);
        
        if (employeeExist)
        {
            throw new ConflictException("Employee already exists");
        }
        
        if (employee != null)
        {
            employee.FullName = updateEmployeeRequestDto.FullName;
            employee.Email = updateEmployeeRequestDto.Email;
            employee.Department = updateEmployeeRequestDto.Department;
        };

        
        await _dbContext.SaveChangesAsync();
        return employee ?? throw new NotFoundException("Employee not found");

    }

    public async Task<bool> DeleteEmployee(int employeeId)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        if (employee == null)
        {
          throw new NotFoundException("Employee not found");  
        }
        
        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<LeaveRequestResponseDto>> GetEmployeeLeaveHistory(int id)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == id);
        
        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }
        
        var leaveHistory = await _dbContext.LeaveRequests
            .Where(lr => lr.EmployeeId == id)
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
        
        if (leaveHistory.Count == 0)
        {
            throw new NotFoundException($"No leave history found for employee: {id}");
        }
        return leaveHistory;
    }

    public async Task<IEnumerable<EmployeeOnLeaveResponseDto>> GetEmployeesOnLeave()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var employees = await _dbContext.Employees
            .Include(e => e.LeaveRequests)
            .Where(e => e.LeaveRequests.Any(lr => 
                lr.Status == LeaveConstants.Approved && 
                lr.StartDate <= today && 
                lr.EndDate >= today))
            .SelectMany(e => e.LeaveRequests
                .Where(lr => lr.Status == LeaveConstants.Approved && 
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
}