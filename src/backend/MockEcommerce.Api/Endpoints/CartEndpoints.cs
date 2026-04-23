using Microsoft.AspNetCore.Http.HttpResults;
using MockEcommerce.Api.Models;
using MockEcommerce.Api.Services;

namespace MockEcommerce.Api.Endpoints;

/// <summary>
/// Maps shopping cart endpoints under <c>/api/cart</c>.
/// </summary>
public static class CartEndpoints
{
    private const int MaxQuantityPerProduct = 5;

    /// <summary>Registers cart-related routes on the given endpoint route builder.</summary>
    public static void MapCartEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/cart")
            .WithTags("Cart");

        group.MapGet("/", GetCart)
            .WithName("GetCart")
            .WithSummary("Returns all items currently in the cart.");

        group.MapPost("/", AddToCart)
            .WithName("AddToCart")
            .WithSummary("Adds a product to the cart or increments quantity if already present.");

        group.MapPut("/{productId:int}", UpdateCartItem)
            .WithName("UpdateCartItem")
            .WithSummary("Updates the quantity of an existing cart item.");

        group.MapDelete("/{productId:int}", RemoveFromCart)
            .WithName("RemoveFromCart")
            .WithSummary("Removes a single product from the cart by its product ID.");

        group.MapDelete("/", ClearCart)
            .WithName("ClearCart")
            .WithSummary("Removes all items from the cart.");
    }

    /// <summary>Returns all items currently in the cart.</summary>
    internal static Ok<CartResponse> GetCart(ICartService cartService)
    {
        var items = cartService.GetAll().ToList();
        var response = new CartResponse
        {
            Items = items,
            CartTotal = items.Sum(i => i.TotalPrice)
        };
        return TypedResults.Ok(response);
    }

    /// <summary>Adds a product to the cart or increments quantity if already present.</summary>
    internal static Results<Created<CartItem>, Ok<CartItem>, NotFound<string>, ValidationProblem> AddToCart(
        AddToCartRequest request,
        IProductService productService,
        ICartService cartService)
    {
        // Validate quantity range
        if (request.Quantity < 1 || request.Quantity > MaxQuantityPerProduct)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["quantity"] = [$"Quantity must be between 1 and {MaxQuantityPerProduct}."]
            });
        }

        // Validate product exists
        var product = productService.GetById(request.ProductId);
        if (product is null)
        {
            return TypedResults.NotFound($"Product with ID {request.ProductId} not found.");
        }

        // Check if adding would exceed max quantity
        var existing = cartService.GetByProductId(request.ProductId);
        if (existing is not null)
        {
            var newTotal = existing.Quantity + request.Quantity;
            if (newTotal > MaxQuantityPerProduct)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["quantity"] = [$"Quantity for this product cannot exceed {MaxQuantityPerProduct}. Currently in cart: {existing.Quantity}."]
                });
            }
        }

        var cartItem = new CartItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            UnitPrice = product.Price,
            Quantity = request.Quantity
        };

        var result = cartService.Add(cartItem);

        if (existing is not null)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.Created($"/api/cart/{result.ProductId}", result);
    }

    /// <summary>Updates the quantity of an existing cart item.</summary>
    internal static Results<Ok<CartItem>, NotFound, ValidationProblem> UpdateCartItem(
        int productId,
        UpdateCartItemRequest request,
        ICartService cartService)
    {
        // Validate quantity range
        if (request.Quantity < 1 || request.Quantity > MaxQuantityPerProduct)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["quantity"] = [$"Quantity must be between 1 and {MaxQuantityPerProduct}."]
            });
        }

        var updated = cartService.Update(productId, request.Quantity);
        if (updated is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(updated);
    }

    /// <summary>Removes a single product from the cart by its product ID.</summary>
    internal static Results<NoContent, NotFound> RemoveFromCart(int productId, ICartService cartService)
    {
        var removed = cartService.Remove(productId);
        if (!removed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }

    /// <summary>Removes all items from the cart.</summary>
    internal static NoContent ClearCart(ICartService cartService)
    {
        cartService.Clear();
        return TypedResults.NoContent();
    }
}

/// <summary>Request body for adding a product to the cart.</summary>
public record AddToCartRequest(int ProductId, int Quantity);

/// <summary>Request body for updating a cart item's quantity.</summary>
public record UpdateCartItemRequest(int Quantity);
