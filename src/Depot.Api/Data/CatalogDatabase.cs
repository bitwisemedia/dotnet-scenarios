using Microsoft.Data.Sqlite;

namespace Depot.Api.Data;

/// <summary>
/// Owns the catalogue's SQLite connection string and the one-time schema and
/// seed bootstrap. Connections are short-lived: callers open one per unit of
/// work and dispose it, letting SQLite pool the underlying handle.
/// </summary>
public sealed class CatalogDatabase
{
    private readonly string _connectionString;

    public CatalogDatabase(string dataSource)
    {
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = dataSource,
        }.ToString();
    }

    /// <summary>Opens a new connection to the catalogue database.</summary>
    public SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Creates the schema if it does not exist and seeds a starter catalogue the
    /// first time the service runs against an empty database.
    /// </summary>
    public void EnsureSeeded()
    {
        using var connection = OpenConnection();

        using (var schema = connection.CreateCommand())
        {
            schema.CommandText =
                """
                CREATE TABLE IF NOT EXISTS Parts (
                    AssetTag TEXT PRIMARY KEY,
                    Name     TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Location TEXT NOT NULL,
                    Quantity INTEGER NOT NULL
                );
                """;
            schema.ExecuteNonQuery();
        }

        using (var count = connection.CreateCommand())
        {
            count.CommandText = "SELECT COUNT(*) FROM Parts";
            if (Convert.ToInt64(count.ExecuteScalar()) > 0)
            {
                return;
            }
        }

        foreach (var (tag, name, category, location, quantity) in SeedRows)
        {
            using var insert = connection.CreateCommand();
            insert.CommandText =
                "INSERT INTO Parts (AssetTag, Name, Category, Location, Quantity) " +
                "VALUES ($tag, $name, $category, $location, $quantity)";
            insert.Parameters.AddWithValue("$tag", tag);
            insert.Parameters.AddWithValue("$name", name);
            insert.Parameters.AddWithValue("$category", category);
            insert.Parameters.AddWithValue("$location", location);
            insert.Parameters.AddWithValue("$quantity", quantity);
            insert.ExecuteNonQuery();
        }
    }

    private static readonly (string Tag, string Name, string Category, string Location, int Quantity)[] SeedRows =
    [
        ("LAP-0417", "Dell Latitude 7450", "laptop", "A1-03", 1),
        ("LAP-0418", "Dell Latitude 7450", "laptop", "A1-03", 1),
        ("NET-2210", "Meraki MS220 switch", "networking", "C4-11", 4),
        ("PWR-0099", "APC UPS 1500VA", "power", "C4-02", 2),
        ("CBL-3301", "Cat6 patch cable 2m", "cabling", "B2-07", 250),
    ];
}
