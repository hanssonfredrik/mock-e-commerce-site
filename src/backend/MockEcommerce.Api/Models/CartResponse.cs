namespace MockEcommerce.Api.Models;

/// <summary>
/// Response DTO for the GET /api/cart endpoint.
/// </summary>
public class CartResponse
{
    /// <summary>All items currently in the cart.</summary>
    public IEnumerable<CartItem> Items { get; set; } = [];

    /// <summary>Sum of all item totals.</summary>
    public decimal CartTotal { get; set; }
}
