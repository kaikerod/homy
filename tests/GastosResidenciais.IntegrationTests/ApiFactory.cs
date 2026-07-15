using GastosResidenciais.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GastosResidenciais.IntegrationTests;

internal sealed class ApiFactory(string databasePath) : WebApplicationFactory<Program>
{
    public string DatabasePath { get; } = databasePath;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", $"Data Source={DatabasePath}");
        builder.UseSetting("Database:ApplyMigrations", "false");
    }

    public async Task<HttpClient> CreateInitializedClientAsync()
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
        return client;
    }
}

