namespace Neighbor.Contract.Abstractions.Shared;

public sealed class ValidationResult<TValue> : Result<TValue>, IValidationResult
{
    private ValidationResult(Error[] errros)
        : base(default, false, IValidationResult.ValidationError) => Errors = errros;

    public Error[] Errors { get; }

    public static ValidationResult<TValue> WithErrors(Error[] errros) => new(errros);
}
