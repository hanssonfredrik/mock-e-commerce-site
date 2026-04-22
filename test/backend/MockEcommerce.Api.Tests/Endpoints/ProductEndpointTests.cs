using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MockEcommerce.Api.Models;

namespace MockEcommerce.Api.Tests.Endpoints;

public class ProductEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithProducts()
    {
        var response = await _client.GetAsync("/api/products");

        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
        Assert.NotEmpty(products);
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkWithProduct()
    {
        var response = await _client.GetAsync("/api/products/1");

        response.EnsureSuccessStatusCode();
        var product = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(product);
        Assert.Equal(1, product.Id);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/products/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
