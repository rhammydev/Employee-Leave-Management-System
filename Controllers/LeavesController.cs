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
    public async Task<IActionResult> SubmitLeaveRequest(SubmitLeaveRequestDto submitLeaveRequestDto)
    {
        var leaveRequest = await _repository.SubmitLeaveRequest(submitLeaveRequestDto);
        return Ok(leaveRequest);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLeaveRequest(int id, SubmitLeaveRequestDto submitLeaveRequestDto)
    {
        var leaveRequest = await _repository.UpdateLeaveRequest(id, submitLeaveRequestDto);
        return Ok(leaveRequest);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLeaveRequest(int id)
    {
        var leaveRequest = await _repository.DeleteLeaveRequest(id);
        return Ok(leaveRequest);
    }

    [HttpPost("approve/{id}")]
    public async Task<IActionResult> ApproveLeaveRequest(int id, LeaveActionRequestDto leaveActionRequestDto)
    {
        var leaveRequest = await _repository.ApproveLeaveRequest(id, leaveActionRequestDto);
        return Ok(leaveRequest);
    }

    [HttpPost("reject/{id}")]
    public async Task<IActionResult> RejectLeaveRequest(int id, LeaveActionRequestDto  leaveActionRequestDto)
    {
        var leaveRequest = await _repository.RejectLeaveRequest(id, leaveActionRequestDto);
        return Ok(leaveRequest);
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetLeavesStatsByDepartment(string department)
    {
        var leaves = await _repository.GetLeavesStatsByDepartment(department);
        return Ok(leaves);
    }

   

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetLeaveByStatus(string status)
    {
        var leaves = await _repository.GetLeaveByStatus(status);
        return Ok(leaves);
    }
}