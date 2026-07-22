# dotnet-scenarios

Depot is a small internal service for a facilities and IT team to track hardware assets and stocked parts. It exposes a
JSON API over a SQLite-backed catalogue — search the catalogue, look an item up by its asset tag, and stream the
attachments (invoices, warranty documents, import manifests) held for each item in the on-disk document store.

## Usage

Run the API:

```sh
dotnet run --project src/Depot.Api
```

The service creates and seeds its SQLite database on first run, then serves:

| Method & path                        | Description                                             |
| ------------------------------------ | ------------------------------------------------------ |
| `GET /api/parts/search?term=<text>`  | Substring search over the catalogue by item name       |
| `GET /api/parts/{assetTag}`          | Fetch a single item by its stencilled asset tag        |
| `GET /api/documents/{name}`          | Stream a stored attachment by file name                |
| `GET /api/imports/{batchId}/manifest`| Stream the JSON manifest an importer wrote for a batch  |

## Layout

```
src/Depot.Api            ASP.NET Core Web API
  Controllers            HTTP surface (parts, documents)
  Data                   SQLite catalogue: bootstrap + repository
  Storage                on-disk document store
  Models, Options        DTOs and bound configuration
tests/Depot.Tests        xUnit tests
```

## Development

```sh
dotnet build Depot.slnx
dotnet test Depot.slnx
```

CI restores, builds, and tests the solution on every push and pull request; CodeQL analyzes the C# project and the
GitHub Actions workflows on every push and pull request plus a weekly schedule.
