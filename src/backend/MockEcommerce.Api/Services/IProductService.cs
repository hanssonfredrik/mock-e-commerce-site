using MockEcommerce.Api.Models;

namespace MockEcommerce.Api.Services;

/// <summary>
/// Defines operations for reading the product catalog.
/// </summary>
public interface IProductService
{
    /// <summary>Returns all products.</summary>
    IEnumerable<Product> GetAll();

    /// <summary>Returns a product by ID, or <c>null</c> if not found.</summary>
    /// <param name="id">The product ID.</param>
    Product? GetById(int id);
}
