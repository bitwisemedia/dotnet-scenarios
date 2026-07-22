namespace Depot.Api.Options;

/// <summary>
/// Configuration for the on-disk document store bound from the "Storage"
/// section of appsettings.
/// </summary>
public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    /// <summary>
    /// Root directory the document store reads attachments and import manifests
    /// from. Relative paths are resolved against the content root.
    /// </summary>
    public string DocumentRoot { get; set; } = Path.Combine("data", "documents");
}
