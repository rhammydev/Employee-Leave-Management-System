using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.DTOs;

namespace EmployeeLeaveManagementSystem.Repositories;

public interface IEmployeeRepository
{
    public Task<IEnumerable<EmployeeResponseDto>> GetAllEmployees();
    
    public Task<EmployeeResponseDto> GetEmployeeById(int employeeId);
    
    public Task<Employee> CreateEmployee(CreateEmployeeRequestDto createEmployeeRequestDto);
    
    public Task<Employee> UpdateEmployee(int id, UpdateEmployeeRequestDto updateEmployeeRequestDto);
    
    public Task<bool> DeleteEmployee(int employeeId);

    public Task<IEnumerable<LeaveRequestResponseDto>> GetEmployeeLeaveHistory(int id);
    
    public Task<IEnumerable<EmployeeOnLeaveResponseDto>> GetEmployeesOnLeave();


}