namespace EmployeeLeaveManagementSystem.Constants;

public class LeaveConstants
{
    public const string Pending = "PENDING";
    public const string Processing = "PROCESSING";
    public const string Rejected = "REJECTED";
    public const string Approved = "APPROVED";

    public static readonly HashSet<string> ValidStatus = new(StringComparer.OrdinalIgnoreCase)
    {
        Pending,
        Processing,
        Rejected,
        Approved
    };
    
    public static readonly HashSet<string> ValidLeaveType = new(StringComparer.OrdinalIgnoreCase)
    {
        "ANNUAL",
        "SICK",
        "MATERNITY",
        "PATERNITY",
        "UNPAID",
        "EMERGENCY",
        "STUDY"
    };
    
    public static string JoinedStatus => string.Join(", ", ValidStatus);
    public static string JoinedLeaveType => string.Join(", ", ValidLeaveType);
    
}