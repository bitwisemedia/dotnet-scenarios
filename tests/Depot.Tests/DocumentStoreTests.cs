using Depot.Api.Options;
using Depot.Api.Storage;
using Microsoft.Extensions.Options;
using Xunit;

namespace Depot.Tests;

public sealed class DocumentStoreTests : IDisposable
{
    private readonly string _root;
    private readonly DocumentStore _store;

    public DocumentStoreTests()
    {
        _root = Path.Combine(Path.GetTempPath(), $"depot-docs-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(_root, "imports"));
        _store = new DocumentStore(Options.Create(new StorageOptions { DocumentRoot = _root }));
    }

    [Fact]
    public void OpenDocument_ReadsStoredFile()
    {
        File.WriteAllText(Path.Combine(_root, "note.txt"), "hello");

        using var stream = _store.OpenDocument("note.txt");
        using var reader = new StreamReader(stream);

        Assert.Equal("hello", reader.ReadToEnd());
    }

    [Fact]
    public void OpenManifest_RejectsNonGuidBatchId()
    {
        Assert.Throws<ArgumentException>(() => _store.OpenManifest("not-a-guid"));
    }

    [Fact]
    public void OpenManifest_ReadsValidBatch()
    {
        var batchId = "3f2504e0-4f89-41d3-9a0c-0305e82c3301";
        File.WriteAllText(Path.Combine(_root, "imports", batchId + ".json"), "{}");

        using var stream = _store.OpenManifest(batchId);
        using var reader = new StreamReader(stream);

        Assert.Equal("{}", reader.ReadToEnd());
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }
}
