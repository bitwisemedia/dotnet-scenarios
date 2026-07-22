namespace Depot.Api.Models;

/// <summary>
/// A tracked inventory item — a piece of hardware or a stocked part held in one
/// of the depot's storage locations.
/// </summary>
/// <param name="AssetTag">The asset tag stencilled on the item; unique per item.</param>
/// <param name="Name">Human-readable description used by the search surface.</param>
/// <param name="Category">Catalogue grouping (e.g. "laptop", "networking").</param>
/// <param name="Location">Bin or shelf the item currently lives in.</param>
/// <param name="Quantity">On-hand count for consumable parts; 1 for unique assets.</param>
public sealed record Part(
    string AssetTag,
    string Name,
    string Category,
    string Location,
    int Quantity);
