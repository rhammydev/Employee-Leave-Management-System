using EmployeeLeaveManagementSystem.Constants;
using EmployeeLeaveManagementSystem.Models.DTOs;
using FluentValidation;

namespace EmployeeLeaveManagementSystem.Validators;

public class SubmitLeaveRequestValidator : AbstractValidator<SubmitLeaveRequestDto>
{
    public SubmitLeaveRequestValidator()
    {
        RuleFor(leave => leave.EmployeeId).NotEmpty().WithMessage("Employee id is required");
        
        RuleFor(leave => leave.LeaveType).NotEmpty().WithMessage("Leave type is required")
            .Must(l => LeaveConstants.ValidLeaveType.Contains(l.ToUpper()))
            .WithMessage($"Invalid leave type, Leave type must be one of: {LeaveConstants.JoinedLeaveType}");

        RuleFor(leave => leave.StartDate).NotEmpty().WithMessage("Start date is required");
        
        RuleFor(leave => leave.EndDate).NotEmpty().WithMessage("End Date is required");
        
        RuleFor(leave => leave.Reason).NotEmpty().WithMessage("Reason is required")
            .Length(5,150).WithMessage("Reason must be between 5 and 150 characters");
    }
}