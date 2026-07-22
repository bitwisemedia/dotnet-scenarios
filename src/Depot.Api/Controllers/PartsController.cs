using Depot.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Depot.Api.Controllers;

[ApiController]
[Route("api/parts")]
public sealed class PartsController : ControllerBase
{
    private readonly CatalogRepository _catalog;

    public PartsController(CatalogRepository catalog) => _catalog = catalog;

    /// <summary>Substring search over the catalogue used by the operator UI.</summary>
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest("term is required");
        }

        return Ok(_catalog.SearchByName(term));
    }

    /// <summary>Fetches a single item by its asset tag.</summary>
    [HttpGet("{assetTag}")]
    public IActionResult Get(string assetTag)
    {
        var part = _catalog.GetByAssetTag(assetTag);
        return part is null ? NotFound() : Ok(part);
    }
}
