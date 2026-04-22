using MockEcommerce.Api.Models;

namespace MockEcommerce.Api.Services;

/// <summary>
/// Thread-safe in-memory cart storage. Registered as Singleton for demo purposes;
/// all users share a single cart. Replace with a per-user scoped implementation
/// when authentication is added.
/// </summary>
public class InMemoryCartService : ICartService
{
    private readonly List<CartItem> _cart = [];
    private readonly Lock _lock = new();

    /// <inheritdoc />
    public IEnumerable<CartItem> GetAll()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public CartItem? GetByProductId(int productId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public CartItem Add(CartItem item)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool Remove(int productId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Clear()
    {
        throw new NotImplementedException();
    }
}
