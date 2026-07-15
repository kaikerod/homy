using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace GastosResidenciais.IntegrationTests.Controllers;

public sealed class ApiEndpointsTests
{
    [Fact]
    public async Task FluxoCompleto_RespeitaContratosRegrasTotaisECascade()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"gastos-{Guid.NewGuid():N}.db");
        await using var factory = new ApiFactory(databasePath);
        using var client = await factory.CreateInitializedClientAsync();

        var maria = await CriarPessoaAsync(client, "Maria Silva", 34);
        var lucas = await CriarPessoaAsync(client, "Lucas Silva", 15);
        var semTransacoes = await CriarPessoaAsync(client, "Ana", 22);

        var pessoasResponse = await client.GetAsync("/api/v1/pessoas");
        Assert.Equal(HttpStatusCode.OK, pessoasResponse.StatusCode);
        var pessoas = await pessoasResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(3, pessoas.GetArrayLength());

        var inexistenteResponse = await client.PostAsJsonAsync("/api/v1/transacoes", new
        {
            descricao = "Teste",
            valor = 10m,
            tipo = "Despesa",
            pessoaId = Guid.CreateVersion7()
        });
        await AssertErrorAsync(inexistenteResponse, HttpStatusCode.NotFound, "Pessoa não encontrada.");

        var receitaMenorResponse = await client.PostAsJsonAsync("/api/v1/transacoes", new
        {
            descricao = "Mesada",
            valor = 100m,
            tipo = "Receita",
            pessoaId = lucas
        });
        await AssertErrorAsync(
            receitaMenorResponse,
            HttpStatusCode.BadRequest,
            "Pessoas menores de 18 anos só podem ter transações do tipo Despesa.");

        await CriarTransacaoAsync(client, "Salário", 3500m, "Receita", maria);
        await CriarTransacaoAsync(client, "Aluguel", 1200m, "Despesa", maria);
        await CriarTransacaoAsync(client, "Lanche", 100m, "Despesa", lucas);

        var transacoesResponse = await client.GetAsync("/api/v1/transacoes");
        Assert.Equal(HttpStatusCode.OK, transacoesResponse.StatusCode);
        var transacoes = await transacoesResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(3, transacoes.GetArrayLength());
        Assert.All(transacoes.EnumerateArray(), item =>
            Assert.False(string.IsNullOrWhiteSpace(item.GetProperty("pessoaNome").GetString())));

        var totaisResponse = await client.GetAsync("/api/v1/totais");
        Assert.Equal(HttpStatusCode.OK, totaisResponse.StatusCode);
        var totais = await totaisResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(3500m, totais.GetProperty("totalGeralReceitas").GetDecimal());
        Assert.Equal(1300m, totais.GetProperty("totalGeralDespesas").GetDecimal());
        Assert.Equal(2200m, totais.GetProperty("saldoLiquidoGeral").GetDecimal());

        var anaTotal = totais.GetProperty("pessoas")
            .EnumerateArray()
            .Single(item => item.GetProperty("pessoaId").GetGuid() == semTransacoes);
        Assert.Equal(0m, anaTotal.GetProperty("totalReceitas").GetDecimal());
        Assert.Equal(0m, anaTotal.GetProperty("totalDespesas").GetDecimal());
        Assert.Equal(0m, anaTotal.GetProperty("saldo").GetDecimal());

        var deleteResponse = await client.DeleteAsync($"/api/v1/pessoas/{lucas}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        transacoesResponse = await client.GetAsync("/api/v1/transacoes");
        transacoes = await transacoesResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(2, transacoes.GetArrayLength());
        Assert.DoesNotContain(
            transacoes.EnumerateArray(),
            item => item.GetProperty("pessoaId").GetGuid() == lucas);

        var deleteInexistente = await client.DeleteAsync($"/api/v1/pessoas/{lucas}");
        await AssertErrorAsync(deleteInexistente, HttpStatusCode.NotFound, "Pessoa não encontrada.");

        File.Delete(databasePath);
    }

    [Fact]
    public async Task ValidacaoESwagger_UsamContratoPadronizado()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"gastos-{Guid.NewGuid():N}.db");
        await using var factory = new ApiFactory(databasePath);
        using var client = await factory.CreateInitializedClientAsync();

        var invalidResponse = await client.PostAsJsonAsync("/api/v1/pessoas", new
        {
            nome = " ",
            idade = 131
        });
        await AssertErrorAsync(
            invalidResponse,
            HttpStatusCode.BadRequest,
            "Um ou mais campos são inválidos.",
            expectErrors: true);

        var malformedEnum = await client.PostAsJsonAsync("/api/v1/transacoes", new
        {
            descricao = "Teste",
            valor = 10m,
            tipo = "Outro",
            pessoaId = Guid.CreateVersion7()
        });
        await AssertErrorAsync(
            malformedEnum,
            HttpStatusCode.BadRequest,
            "Um ou mais campos são inválidos.",
            expectErrors: true);

        var swagger = await client.GetAsync("/swagger/v1/swagger.json");
        Assert.Equal(HttpStatusCode.OK, swagger.StatusCode);
        var openApi = await swagger.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(openApi.GetProperty("paths").TryGetProperty("/api/v1/pessoas", out _));
        Assert.True(openApi.GetProperty("paths").TryGetProperty("/api/v1/transacoes", out _));
        Assert.True(openApi.GetProperty("paths").TryGetProperty("/api/v1/totais", out _));

        File.Delete(databasePath);
    }

    [Fact]
    public async Task BancoEmArquivo_PersisteDadosEntreReinicializacoes()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"gastos-{Guid.NewGuid():N}.db");

        await using (var firstFactory = new ApiFactory(databasePath))
        {
            using var firstClient = await firstFactory.CreateInitializedClientAsync();
            await CriarPessoaAsync(firstClient, "Persistida", 40);
        }

        await using (var secondFactory = new ApiFactory(databasePath))
        {
            using var secondClient = await secondFactory.CreateInitializedClientAsync();
            var response = await secondClient.GetAsync("/api/v1/pessoas");
            var pessoas = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Single(pessoas.EnumerateArray());
            Assert.Equal("Persistida", pessoas[0].GetProperty("nome").GetString());
        }

        File.Delete(databasePath);
    }

    private static async Task<Guid> CriarPessoaAsync(HttpClient client, string nome, int idade)
    {
        var response = await client.PostAsJsonAsync("/api/v1/pessoas", new { nome, idade });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("id").GetGuid();
    }

    private static async Task CriarTransacaoAsync(
        HttpClient client,
        string descricao,
        decimal valor,
        string tipo,
        Guid pessoaId)
    {
        var response = await client.PostAsJsonAsync("/api/v1/transacoes", new
        {
            descricao,
            valor,
            tipo,
            pessoaId
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    private static async Task AssertErrorAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatus,
        string expectedMessage,
        bool expectErrors = false)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(expectedMessage, error.GetProperty("message").GetString());
        Assert.Equal((int)expectedStatus, error.GetProperty("status").GetInt32());
        Assert.Equal(JsonValueKind.Array, error.GetProperty("errors").ValueKind);
        if (expectErrors)
        {
            Assert.NotEmpty(error.GetProperty("errors").EnumerateArray());
        }
    }
}
