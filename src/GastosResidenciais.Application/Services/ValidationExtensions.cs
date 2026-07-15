using FluentValidation;
using GastosResidenciais.Application.Exceptions;

namespace GastosResidenciais.Application.Services;

internal static class ValidationExtensions
{
    public static async Task ValidateAndThrowAppAsync<T>(
        this IValidator<T> validator,
        T request,
        CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            throw new AppValidationException(result.Errors);
        }
    }
}

