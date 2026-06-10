using EmployeeLeaveManagementSystem.Models.DTOs;
using FluentValidation;

namespace EmployeeLeaveManagementSystem.Validators;

public class RejectLeaveRequestValidation : AbstractValidator<RejectLeaveRequestDto>
{
    public RejectLeaveRequestValidation()
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