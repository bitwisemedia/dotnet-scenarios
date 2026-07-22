using Depot.Api.Data;
using Depot.Api.Options;
using Depot.Api.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection(StorageOptions.SectionName));

builder.Services.AddSingleton(_ =>
{
    var dataSource = builder.Configuration.GetConnectionString("Catalog") ?? "depot.db";
    return new CatalogDatabase(dataSource);
});
builder.Services.AddScoped<CatalogRepository>();
builder.Services.AddSingleton<DocumentStore>();

var app = builder.Build();

// Create the schema and seed a starter catalogue on first run.
app.Services.GetRequiredService<CatalogDatabase>().EnsureSeeded();

app.MapControllers();

app.Run();
