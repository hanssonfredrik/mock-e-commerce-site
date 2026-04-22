namespace MockEcommerce.Api.Models;

/// <summary>
/// Represents a product in the catalog.
/// </summary>
public class Product
{
    /// <summary>Unique product identifier.</summary>
    public int Id { get; set; }

    /// <summary>Display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Detailed product description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Unit price in USD.</summary>
    public decimal Price { get; set; }

    /// <summary>Product category (e.g. "Electronics").</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Number of units available in stock.</summary>
    public int Stock { get; set; }

    /// <summary>URL of the product image.</summary>
    public string ImageUrl { get; set; } = string.Empty;
}
