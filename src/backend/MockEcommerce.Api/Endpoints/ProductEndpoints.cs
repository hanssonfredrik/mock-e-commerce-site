using Microsoft.AspNetCore.Http.HttpResults;
using MockEcommerce.Api.Models;
using MockEcommerce.Api.Services;

namespace MockEcommerce.Api.Endpoints;

/// <summary>
/// Maps product catalog endpoints under <c>/api/products</c>.
/// </summary>
public static class ProductEndpoints
{
    /// <summary>Registers product-related routes on the given endpoint route builder.</summary>
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/products")
            .WithTags("Products");

        group.MapGet("/", GetAll)
            .WithName("GetAllProducts")
            .WithSummary("Returns all products in the catalog.");

        group.MapGet("/{id:int}", GetById)
            .WithName("GetProductById")
            .WithSummary("Returns a single product by its unique identifier.");
    }

    /// <summary>Returns all products in the catalog.</summary>
    internal static Ok<IEnumerable<Product>> GetAll(IProductService productService)
    {
        return TypedResults.Ok(productService.GetAll());
    }

    /// <summary>Returns a single product by its unique identifier.</summary>
    internal static Results<Ok<Product>, NotFound> GetById(int id, IProductService productService)
    {
        var product = productService.GetById(id);
        if (product is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(product);
    }
}
