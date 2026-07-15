using GastosResidenciais.API.Contracts;
using GastosResidenciais.Application.DTOs.Pessoas;
using GastosResidenciais.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.API.Controllers;

[ApiController]
[Route("api/v1/pessoas")]
[Produces("application/json")]
public sealed class PessoasController(IPessoaService service) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PessoaResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PessoaResponse>> Criar(
        CreatePessoaRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.CriarAsync(request, cancellationToken);
        return Created($"/api/v1/pessoas/{response.Id}", response);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<PessoaResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<PessoaResponse>>> Listar(
        CancellationToken cancellationToken) =>
        Ok(await service.ListarAsync(cancellationToken));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken cancellationToken)
    {
        await service.ExcluirAsync(id, cancellationToken);
        return NoContent();
    }
}
