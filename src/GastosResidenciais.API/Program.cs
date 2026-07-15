using System.Text.Json.Serialization;
using GastosResidenciais.API.Contracts;
using GastosResidenciais.API.Extensions;
using GastosResidenciais.API.Middlewares;
using GastosResidenciais.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/gastos-.log", rollingInterval: RollingInterval.Day));

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(entry => entry.Errors)
            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                ? "O corpo da requisição é inválido."
                : error.ErrorMessage)
            .Distinct()
            .ToArray();

        return new BadRequestObjectResult(new ErrorResponse(
            "Um ou mais campos são inválidos.",
            errors,
            StatusCodes.Status400BadRequest));
    };
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Gastos Residenciais API",
        Version = "v1",
        Description = "API para controle de receitas e despesas residenciais."
    });
});

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
{
    if (allowedOrigins.Length > 0)
    {
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    }
}));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;
    if (response.StatusCode >= 400 && response.ContentLength is null)
    {
        response.ContentType = "application/json";
        await response.WriteAsJsonAsync(new ErrorResponse(
            response.StatusCode == StatusCodes.Status404NotFound
                ? "Recurso não encontrado."
                : "Não foi possível processar a requisição.",
            [],
            response.StatusCode));
    }
});

app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gastos Residenciais API v1"));
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.MapControllers();

if (builder.Configuration.GetValue("Database:ApplyMigrations", true))
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

await app.RunAsync();

public partial class Program;

