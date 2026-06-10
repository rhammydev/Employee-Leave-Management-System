using EmployeeLeaveManagementSystem.Constants;
using EmployeeLeaveManagementSystem.Models.DTOs;
using FluentValidation;

namespace EmployeeLeaveManagementSystem.Validators;

public class ApproveLeaveRequestValidation : AbstractValidator<ApproveLeaveRequestDto>
{
    public ApproveLeaveRequestValidation()
    {
        RuleFor(l => l.ApproverId)
            .NotEmpty()
            .WithMessage("Approver Id is required");
        
    }
}