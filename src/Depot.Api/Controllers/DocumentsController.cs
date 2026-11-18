using Depot.Api.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Depot.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class DocumentsController : ControllerBase
{
    private readonly DocumentStore _store;

    public DocumentsController(DocumentStore store) => _store = store;

    /// <summary>Streams a stored attachment back to the caller.</summary>
    [HttpGet("documents/{name}")]
    public IActionResult Download(string name)
    {
        try
        {
            var stream = _store.OpenDocument(name);
            return File(stream, "application/octet-stream", name);
        }
        catch (ArgumentException)
        {
            return BadRequest("invalid document name");
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Streams the manifest for a completed import batch.</summary>
    [HttpGet("imports/{batchId}/manifest")]
    public IActionResult Manifest(string batchId)
    {
        try
        {
            var stream = _store.OpenManifest(batchId);
            return File(stream, "application/json");
        }
        catch (ArgumentException)
        {
            return BadRequest("invalid batch id");
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }
}
