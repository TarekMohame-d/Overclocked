using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.EmployeeActivityLogAggregate.ValueObjects;

public record EmployeeActivityLogId(Guid Value) : IEntityKey
{
    public static EmployeeActivityLogId Create() => new(Guid.CreateVersion7());

    public static EmployeeActivityLogId Create(Guid value) => new(value);
}
