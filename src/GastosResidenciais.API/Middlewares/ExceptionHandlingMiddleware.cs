using GastosResidenciais.API.Contracts;
using GastosResidenciais.Application.Exceptions;
using GastosResidenciais.Domain.Exceptions;

namespace GastosResidenciais.API.Middlewares;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppValidationException exception)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status400BadRequest,
                exception.Message,
                exception.Errors);
        }
        catch (BusinessRuleException exception)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, exception.Message);
        }
        catch (NotFoundException exception)
        {
            await WriteErrorAsync(context, StatusCodes.Status404NotFound, exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Erro não tratado ao processar {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Ocorreu um erro interno. Tente novamente mais tarde.");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int status,
        string message,
        IReadOnlyCollection<string>? errors = null)
    {
        if (context.Response.HasStarted)
        {
            throw new InvalidOperationException("A resposta HTTP já foi iniciada.");
        }

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(
            new ErrorResponse(message, errors ?? [], status),
            cancellationToken: context.RequestAborted);
    }
}
