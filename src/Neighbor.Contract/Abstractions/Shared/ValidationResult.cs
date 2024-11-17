namespace Neighbor.Contract.Abstractions.Shared;

public sealed class ValidationResult : Result, IValidationResult
{
    private ValidationResult(Error[] errros)
        : base(false, IValidationResult.ValidationError) => Errors = errros;

    public Error[] Errors { get; }

    public static ValidationResult WithErrors(Error[] errros) => new(errros);
}
