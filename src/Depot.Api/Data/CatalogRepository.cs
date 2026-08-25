using Depot.Api.Models;

namespace Depot.Api.Data;

/// <summary>
/// Read access to the parts catalogue. Backed by ADO.NET over the shared
/// <see cref="CatalogDatabase"/> connection string.
/// </summary>
public sealed class CatalogRepository
{
    private readonly CatalogDatabase _database;

    public CatalogRepository(CatalogDatabase database) => _database = database;

    /// <summary>
    /// Returns catalogue entries whose name contains <paramref name="term"/>,
    /// ordered alphabetically. Used by the operator search box, which does a
    /// substring match as the user types.
    /// </summary>
    public IReadOnlyList<Part> SearchByName(string term)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT AssetTag, Name, Category, Location, Quantity FROM Parts " +
            "WHERE Name LIKE @namePattern " +
            "ORDER BY Name";
        AddParameter(command, "@namePattern", "%" + term + "%");

        return ReadParts(command);
    }

    /// <summary>
    /// Looks up a single item by its stencilled asset tag, or <c>null</c> when
    /// no item carries that tag.
    /// </summary>
    public Part? GetByAssetTag(string assetTag)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT AssetTag, Name, Category, Location, Quantity FROM Parts " +
            "WHERE AssetTag = @assetTag";
        AddParameter(command, "@assetTag", assetTag);

        return ReadParts(command).FirstOrDefault();
    }

    private static void AddParameter(System.Data.IDbCommand command, string name, string value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    private static List<Part> ReadParts(System.Data.IDbCommand command)
    {
        using var reader = command.ExecuteReader();
        var parts = new List<Part>();
        while (reader.Read())
        {
            parts.Add(new Part(
                AssetTag: reader.GetString(0),
                Name: reader.GetString(1),
                Category: reader.GetString(2),
                Location: reader.GetString(3),
                Quantity: reader.GetInt32(4)));
        }

        return parts;
    }
}
