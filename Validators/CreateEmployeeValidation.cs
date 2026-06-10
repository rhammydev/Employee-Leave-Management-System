using EmployeeLeaveManagementSystem.Constants;
using EmployeeLeaveManagementSystem.Models.DTOs;
using FluentValidation;

namespace EmployeeLeaveManagementSystem.Validators;

public class CreateEmployeeValidation : AbstractValidator<CreateEmployeeRequestDto>
{
    public CreateEmployeeValidation()
    {
        RuleFor(e => e.FullName).NotEmpty().Length(3,100)
            .WithMessage("Full name is required and must have between 3 and 100 characters");
        
        RuleFor(e => e.Email).NotEmpty().EmailAddress().WithMessage("A valid email is required");

        RuleFor(e => e.Department).NotEmpty().WithMessage("Department is required")
            .Must(d => DepartmentConstants.Valid.Contains(d.ToUpper()))
            .WithMessage($"Department must be one of: {DepartmentConstants.Joined}");
    }
}