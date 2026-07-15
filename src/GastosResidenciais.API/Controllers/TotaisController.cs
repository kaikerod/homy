using GastosResidenciais.Application.DTOs.Totais;
using GastosResidenciais.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.API.Controllers;

[ApiController]
[Route("api/v1/totais")]
[Produces("application/json")]
public sealed class TotaisController(ITotaisService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<TotaisResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<TotaisResponse>> Obter(CancellationToken cancellationToken) =>
        Ok(await service.ObterAsync(cancellationToken));
}

