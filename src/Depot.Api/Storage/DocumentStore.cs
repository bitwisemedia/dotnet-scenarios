using System.Text.RegularExpressions;
using Depot.Api.Options;
using Microsoft.Extensions.Options;

namespace Depot.Api.Storage;

/// <summary>
/// Serves attachments and import manifests from the on-disk document store.
/// Every item lives under <see cref="StorageOptions.DocumentRoot"/>.
/// </summary>
public sealed partial class DocumentStore
{
    private readonly string _root;

    public DocumentStore(IOptions<StorageOptions> options) => _root = options.Value.DocumentRoot;

    /// <summary>
    /// Opens a stored attachment for reading. <paramref name="name"/> is the file
    /// name as it was registered against the part (e.g. "warranty-LAP-0417.pdf").
    /// </summary>
    public Stream OpenDocument(string name)
    {
        var path = Path.Combine(_root, name);
        return File.OpenRead(path);
    }

    /// <summary>
    /// Opens the JSON manifest an importer writes when it finishes a batch.
    /// <paramref name="batchId"/> is the canonical GUID the importer assigns to
    /// the run; it is validated against the GUID grammar before it is used, so it
    /// can only ever be hex digits and dashes — no directory separators and no
    /// traversal segments can reach the path.
    /// </summary>
    public Stream OpenManifest(string batchId)
    {
        if (!BatchIdPattern().IsMatch(batchId))
        {
            throw new ArgumentException("batch id is not a valid GUID", nameof(batchId));
        }

        var path = Path.Combine(_root, "imports", batchId + ".json");
        return File.OpenRead(path);
    }

    [GeneratedRegex("^[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}$")]
    private static partial Regex BatchIdPattern();
}
