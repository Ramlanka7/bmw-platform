using BmwSalesPlatform.Contracts;
using BmwSalesPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BmwSalesPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISalesService _service;
    public SalesController(ISalesService service) => _service = service;

    [HttpGet("years")]
    [ProducesResponseType(typeof(IEnumerable<YearTotalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetYearTotals(CancellationToken ct)
        => Ok(await _service.GetYearTotalsAsync(ct));

    [HttpGet("models/top")]
    [ProducesResponseType(typeof(PagedResult<TopModelDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopModels([FromQuery] short? year, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        => Ok(await _service.GetTopModelsAsync(year, page, pageSize, ct));

    [HttpGet("regions/mix")]
    [ProducesResponseType(typeof(IEnumerable<RegionalMixDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRegionalMix([FromQuery] short? year, CancellationToken ct)
        => Ok(await _service.GetRegionalMixAsync(year, ct));
}
