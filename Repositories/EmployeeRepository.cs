using EmployeeLeaveManagementSystem.Data;
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
    
    public async Task<IEnumerable<Employee>> GetAllEmployees()
    {
        return await _dbContext.Employees.ToListAsync();
    }

    public async Task<Employee> GetEmployeeById(int employeeId)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        return employee ?? throw new Exception("Employee not found");
    }

    public async Task<Employee> CreateEmployee(CreateEmployeeDto createEmployeeDto)
    {
        var employeeExist = await _dbContext.Employees.AnyAsync(e => e.Email.ToLower() == createEmployeeDto.Email.ToLower());
        if (employeeExist)
        {
            throw new Exception("Employee already exists");
        }

        var employee = new Employee()
        {
            FullName = createEmployeeDto.FullName,
            Email = createEmployeeDto.Email,
            Department = createEmployeeDto.Department,
        };
        
        await _dbContext.Employees.AddAsync(employee);
        await _dbContext.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateEmployee(int id, CreateEmployeeDto createEmployeeDto)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);
        
        var employeeExist = await _dbContext.Employees.AnyAsync(e => e.Email.ToLower() == createEmployeeDto.Email.ToLower());
        if (employeeExist)
        {
            throw new Exception("Employee already exists");
        }
        
        if (employee != null)
        {
            employee.FullName = createEmployeeDto.FullName;
            employee.Email = createEmployeeDto.Email;
            employee.Department = createEmployeeDto.Department;
        };

        
        await _dbContext.SaveChangesAsync();
        return employee ?? throw new Exception("Employee not found");

    }

    public async Task<bool> DeleteEmployee(int employeeId)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        if (employee == null)
        {
          throw new Exception("Employee not found");  
        }
        
        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<LeaveRequest>> GetEmployeeLeaveHistory(int id)
    {
        var leaveHistory = await _dbContext.LeaveRequests.Where(lr => lr.Id == id).ToListAsync();
        if (leaveHistory.Count == 0)
        {
            throw new Exception($"No leave history found for employee: {id}");
        }
        return leaveHistory;
    }
}