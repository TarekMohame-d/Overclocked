namespace Overclocked.SharedKernel;

public sealed record ValidationError : Error
{
    public ValidationError(Dictionary<string, string[]> errors)
        : base("Validation.Error", "One or more validation errors occurred", ErrorType.Validation) => Errors = errors;

    public Dictionary<string, string[]> Errors { get; }
}
