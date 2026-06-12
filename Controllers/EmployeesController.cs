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
    public async Task<IActionResult> CreateEmployee(CreateEmployeeRequestDto createEmployeeRequestDto)
    {
        var employee = await  _repository.CreateEmployee(createEmployeeRequestDto);
        return Ok(employee);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeRequestDto updateEmployeeRequestDto)
    {
        var employee = await _repository.UpdateEmployee(id, updateEmployeeRequestDto);
        return Ok(employee);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _repository.DeleteEmployee(id);
        return Ok(employee);
    }
    
    [HttpGet("on-leave")]
    public async Task<IActionResult> GetEmployeesOnLeave()
    {
        var leaves = await _repository.GetEmployeesOnLeave();
        return Ok(leaves);
    }

    [HttpGet("{id}/leaves")]
    public async Task<IActionResult> GetEmployeeLeaveHistory(int id)
    {
        var leaveRequests = await _repository.GetEmployeeLeaveHistory(id);
        return Ok(leaveRequests);
    }
}