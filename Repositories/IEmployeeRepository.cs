using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.DTOs;

namespace EmployeeLeaveManagementSystem.Repositories;

public interface IEmployeeRepository
{
    public Task<IEnumerable<Employee>> GetAllEmployees();
    
    public Task<Employee> GetEmployeeById(int employeeId);
    
    public Task<Employee> CreateEmployee(CreateEmployeeDto createEmployeeDto);
    
    public Task<Employee> UpdateEmployee(int id, CreateEmployeeDto createEmployeeDto);
    
    public Task<bool> DeleteEmployee(int employeeId);

    public Task<IEnumerable<LeaveRequest>> GetEmployeeLeaveHistory(int id);


}