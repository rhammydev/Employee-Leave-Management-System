using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.DTOs;
using EmployeeLeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeavesController : ControllerBase
{
    private readonly ILeaveRepository _repository;

    public LeavesController(ILeaveRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLeaveRequests()
    {
        var leaveRequests = await _repository.GetAllLeaveRequests();
        return Ok(leaveRequests);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLeaveRequestById(int id)
    {
        var leaveRequest = await _repository.GetLeaveRequestById(id);
        return Ok(leaveRequest);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitLeaveRequest(LeaveRequestDto leaveRequestDto)
    {
        var leaveRequest = await _repository.SubmitLeaveRequest(leaveRequestDto);
        return Ok(leaveRequest);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLeaveRequest(int id, LeaveRequestDto leaveRequestDto)
    {
        var leaveRequest = await _repository.UpdateLeaveRequest(id, leaveRequestDto);
        return Ok(leaveRequest);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLeaveRequest(int id)
    {
        var leaveRequest = await _repository.DeleteLeaveRequest(id);
        return Ok(leaveRequest);
    }

    [HttpPatch("{id}/approve")]
    public async Task<IActionResult> ApproveLeaveRequest(int id)
    {
        var leaveRequest = await _repository.ApproveLeaveRequest(id);
        return Ok(leaveRequest);
    }

    [HttpPatch("{id}/reject")]
    public async Task<IActionResult> RejectLeaveRequest(int id, LeaveRejectDto leaveRejectDto)
    {
        var leaveRequest = await _repository.RejectLeaveRequest(id, leaveRejectDto);
        return Ok(leaveRequest);
    }

    [HttpGet("{department}/department")]
    public async Task<IActionResult> GetDepartmentByDepartment(string department)
    {
        var leaves = await _repository.GetLeavesStatsByDepartment(department);
        return Ok(leaves);
    }

    [HttpGet("employees-on-leave")]
    public async Task<IActionResult> GetEmployeesOnLeave()
    {
        var leaves = await _repository.GetEmployeesOnLeave();
        return Ok(leaves);
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetLeaveByStatus(string status)
    {
        var leaves = await _repository.GetLeaveByStatus(status);
        return Ok(leaves);
    }
}