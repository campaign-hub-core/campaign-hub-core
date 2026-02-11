using CampaignHub.Application.DTOs;
using CampaignHub.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignHub.API.Controllers;

[ApiController]
[Route("api/metasync")]
[Authorize]
public class MetaSyncController : ControllerBase
{
    private readonly MetaAdsSyncService _syncService;

    public MetaSyncController(MetaAdsSyncService syncService)
    {
        _syncService = syncService;
    }

    /// <summary>
    /// Dispara a sincronização completa de uma conta de anúncios com o Meta Ads.
    /// Importa campanhas, ad sets, ads e métricas (últimos 30 dias).
    /// </summary>
    [HttpPost("{adAccountId}/sync")]
    [ProducesResponseType(typeof(SyncResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Sync(string adAccountId, CancellationToken cancellationToken)
    {
        var result = await _syncService.SyncAdAccountAsync(adAccountId, cancellationToken);
        return Ok(result);
    }
}
