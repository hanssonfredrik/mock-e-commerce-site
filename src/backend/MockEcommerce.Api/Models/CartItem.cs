namespace MockEcommerce.Api.Models;

/// <summary>
/// Represents a product entry in the shopping cart.
/// </summary>
public class CartItem
{
    /// <summary>ID of the associated product.</summary>
    public int ProductId { get; set; }

    /// <summary>Snapshot of the product name at time of adding.</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>Price per unit at time of adding.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Number of units in the cart.</summary>
    public int Quantity { get; set; }

    /// <summary>Computed total: <see cref="UnitPrice"/> × <see cref="Quantity"/>.</summary>
    public decimal TotalPrice => UnitPrice * Quantity;
}
