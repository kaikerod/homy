using FluentValidation.Results;

namespace GastosResidenciais.Application.Exceptions;

public sealed class AppValidationException : Exception
{
    public AppValidationException(IEnumerable<ValidationFailure> failures)
        : base("Um ou mais campos são inválidos.")
    {
        Errors = failures.Select(failure => failure.ErrorMessage).Distinct().ToArray();
    }

    public IReadOnlyCollection<string> Errors { get; }
}

