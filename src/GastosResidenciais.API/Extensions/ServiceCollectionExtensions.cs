using FluentValidation;
using GastosResidenciais.Application.DTOs.Pessoas;
using GastosResidenciais.Application.DTOs.Transacoes;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Application.Services;
using GastosResidenciais.Application.Validators;
using GastosResidenciais.Infrastructure.Persistence;
using GastosResidenciais.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPessoaService, PessoaService>();
        services.AddScoped<ITransacaoService, TransacaoService>();
        services.AddScoped<ITotaisService, TotaisService>();
        services.AddScoped<IValidator<CreatePessoaRequest>, CreatePessoaRequestValidator>();
        services.AddScoped<IValidator<CreateTransacaoRequest>, CreateTransacaoRequestValidator>();
        return services;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "A connection string 'DefaultConnection' não foi configurada.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IPessoaRepository, PessoaRepository>();
        services.AddScoped<ITransacaoRepository, TransacaoRepository>();
        return services;
    }
}

