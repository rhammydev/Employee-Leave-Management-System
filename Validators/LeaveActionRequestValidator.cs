using EmployeeLeaveManagementSystem.Constants;
using EmployeeLeaveManagementSystem.Models.DTOs;
using FluentValidation;

namespace EmployeeLeaveManagementSystem.Validators;

public class LeaveActionRequestValidator : AbstractValidator<LeaveActionRequestDto>
{
    public LeaveActionRequestValidator()
    {
        RuleFor(l => l.ApproverId)
            .NotEmpty()
            .WithMessage("Approver Id is required");
        RuleFor(x => x.Reason)
            .NotEmpty()
            .MinimumLength(5)
            .WithMessage("Reason is required");
        
        
    }
}