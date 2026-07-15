using GastosResidenciais.API.Contracts;
using GastosResidenciais.Application.DTOs.Transacoes;
using GastosResidenciais.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.API.Controllers;

[ApiController]
[Route("api/v1/transacoes")]
[Produces("application/json")]
public sealed class TransacoesController(ITransacaoService service) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<TransacaoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransacaoResponse>> Criar(
        CreateTransacaoRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.CriarAsync(request, cancellationToken);
        return Created($"/api/v1/transacoes/{response.Id}", response);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<TransacaoResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TransacaoResponse>>> Listar(
        CancellationToken cancellationToken) =>
        Ok(await service.ListarAsync(cancellationToken));
}
