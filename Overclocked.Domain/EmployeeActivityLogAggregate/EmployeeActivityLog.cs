using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.EmployeeActivityLogAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Domain.EmployeeActivityLogAggregate;

public class EmployeeActivityLog : AggregateRoot<EmployeeActivityLogId>
{
    public UserId EmployeeId { get; private set; }
    public string Action { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private EmployeeActivityLog()
    {
    }

    private EmployeeActivityLog(UserId employeeId, string action)
    {
        EmployeeId = employeeId;
        Action = action;
        CreatedAt = DateTime.UtcNow;
    }

    public static EmployeeActivityLog Create(UserId employeeId, string action)
    {
        return new(employeeId, action);
    }
}
