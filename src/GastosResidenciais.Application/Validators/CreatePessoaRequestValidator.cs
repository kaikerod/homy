using FluentValidation;
using GastosResidenciais.Application.DTOs.Pessoas;

namespace GastosResidenciais.Application.Validators;

public sealed class CreatePessoaRequestValidator : AbstractValidator<CreatePessoaRequest>
{
    public CreatePessoaRequestValidator()
    {
        RuleFor(request => request.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres.");

        RuleFor(request => request.Idade)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Idade é obrigatória.")
            .Must(idade => idade is >= 0 and <= 130)
            .WithMessage("Idade deve estar entre 0 e 130.");
    }
}
