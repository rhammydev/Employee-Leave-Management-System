using EmployeeLeaveManagementSystem.Constants;
using EmployeeLeaveManagementSystem.Models.DTOs;
using FluentValidation;

namespace EmployeeLeaveManagementSystem.Validators;

public class ApproveLeaveRequestValidator : AbstractValidator<LeaveActionRequestDto>
{
    public ApproveLeaveRequestValidator()
    {
        RuleFor(l => l.ApproverId)
            .NotEmpty()
            .WithMessage("Approver Id is required");
        
    }
}