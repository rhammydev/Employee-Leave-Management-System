using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.DTOs;
using EmployeeLeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController: ControllerBase
{
    private readonly IEmployeeRepository _repository;
    public EmployeesController(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEmployees()
    {
        var employees = await _repository.GetAllEmployees();
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var employee = await _repository.GetEmployeeById(id);
        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeDto createEmployeeDto)
    {
        var employee = await  _repository.CreateEmployee(createEmployeeDto);
        return Ok(employee);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, CreateEmployeeDto createEmployeeDto)
    {
        var employee = await _repository.UpdateEmployee(id, createEmployeeDto);
        return Ok(employee);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _repository.DeleteEmployee(id);
        return Ok(employee);
    }

    [HttpGet("{id}/leaves")]
    public async Task<IActionResult> GetEmployeeLeaveHistory(int id)
    {
        var leaveRequests = await _repository.GetEmployeeLeaveHistory(id);
        return Ok(leaveRequests);
    }
}