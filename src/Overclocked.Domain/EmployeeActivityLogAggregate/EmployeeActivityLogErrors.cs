using Overclocked.SharedKernel;

namespace Overclocked.Domain.EmployeeActivityLogAggregate;

public static class EmployeeActivityLogErrors
{
    public static readonly Error ActionCannotBeEmpty = Error.BadRequest(
        "EmployeeActivityLog.ActionCannotBeEmpty",
        "Action cannot be empty."
    );
}
