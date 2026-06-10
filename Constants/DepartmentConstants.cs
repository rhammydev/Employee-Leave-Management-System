namespace EmployeeLeaveManagementSystem.Constants;

public class DepartmentConstants
{
    public static readonly HashSet<string> Valid = new(StringComparer.OrdinalIgnoreCase)
    {
        "RESEARCH",
        "ENGINEERING",
        "PRODUCT",
        "DESIGN",
        "QUALITY ASSURANCE",
        "INFORMATION TECHNOLOGY",
        "DATA ANALYTICS",
        "BUSINESS DEVELOPMENT",
        "MARKETING",
        "CUSTOMER SUPPORT",
        "HUMAN RESOURCES",
        "FINANCE",
        "COMPLIANCE",
        "OPERATIONS",
    };
    
    public static string Joined => string.Join(", ", Valid);
}