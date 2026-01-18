using Overclocked.Domain.EmployeeActivityLogAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.EmployeeActivityLogAggregate;

public sealed class EmployeeActivityLog : AggregateRoot<EmployeeActivityLogId>
{
    public UserId EmployeeId { get; private set; } = null!;
    public string Action { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    private EmployeeActivityLog() { }

    private EmployeeActivityLog(EmployeeActivityLogId id, UserId employeeId, string action)
        : base(id)
    {
        EmployeeId = employeeId;
        Action = action;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Result<EmployeeActivityLog> Create(UserId employeeId, string action)
    {
        if (string.IsNullOrWhiteSpace(action))
            return Result.Failure<EmployeeActivityLog>(EmployeeActivityLogErrors.ActionCannotBeEmpty);

        var employeeActivityLog = new EmployeeActivityLog(EmployeeActivityLogId.Create(), employeeId, action);

        return Result.Success(employeeActivityLog);
    }
}
