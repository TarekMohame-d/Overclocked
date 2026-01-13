using System.Diagnostics.CodeAnalysis;

namespace Overclocked.SharedKernel;

public class Result<TValue>(TValue? value, bool isSuccess, Error error) : Result(isSuccess, error)
{
    [NotNull]
    public TValue Value
    {
        get => IsSuccess ? field! : throw new InvalidOperationException("The value of a failure result can't be accessed.");
    } = value;

    public static new Result<TValue> ValidationFailure(Dictionary<string, string[]> errors) =>
        new(default, false, new ValidationError(errors));
}
