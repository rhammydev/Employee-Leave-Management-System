using EmployeeLeaveManagementSystem.Constants;
using EmployeeLeaveManagementSystem.Models.DTOs;
using FluentValidation;

namespace EmployeeLeaveManagementSystem.Validators;

public class LeaveApprovalValidation : AbstractValidator<LeaveActionRequestDto>
{
    public LeaveApprovalValidation()
    {
        RuleFor(l => l.LeaveRequestId)
            .NotEmpty()
            .WithMessage("Leave Request Id is required");
        
        RuleFor(l => l.ApproverId)
            .NotEmpty()
            .WithMessage("Approver Id is required");
        
        RuleFor(l => l.Action)
            .NotEmpty()
            .WithMessage("Action is required")
            .Must(l => LeaveConstants.ValidStatus.Contains(l.ToUpper()))
            .WithMessage($"Action must be one of {string.Join(", ", LeaveConstants.JoinedStatus)}");

        RuleFor(l => l.Reason)
            .NotEmpty()
            .WithMessage("Reason is required when rejecting a leave request")
            .When(l => string.Equals(
                l.Action,
                LeaveConstants.Rejected,
                StringComparison.OrdinalIgnoreCase));
    }
}