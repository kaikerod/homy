using FluentValidation;
using GastosResidenciais.Application.DTOs.Transacoes;

namespace GastosResidenciais.Application.Validators;

public sealed class CreateTransacaoRequestValidator : AbstractValidator<CreateTransacaoRequest>
{
    public CreateTransacaoRequestValidator()
    {
        RuleFor(request => request.Descricao)
            .NotEmpty().WithMessage("Descrição é obrigatória.")
            .MaximumLength(200).WithMessage("Descrição deve ter no máximo 200 caracteres.");

        RuleFor(request => request.Valor)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Valor deve ser maior que zero.")
            .Must(valor => valor > 0).WithMessage("Valor deve ser maior que zero.");

        RuleFor(request => request.Tipo)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Tipo deve ser 'Receita' ou 'Despesa'.")
            .Must(tipo => tipo.HasValue && Enum.IsDefined(tipo.Value))
            .WithMessage("Tipo deve ser 'Receita' ou 'Despesa'.");

        RuleFor(request => request.PessoaId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Pessoa é obrigatória.")
            .Must(pessoaId => pessoaId != Guid.Empty).WithMessage("Pessoa é obrigatória.");
    }
}
