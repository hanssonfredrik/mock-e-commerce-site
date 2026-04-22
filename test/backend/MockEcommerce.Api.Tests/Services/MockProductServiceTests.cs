using MockEcommerce.Api.Services;

namespace MockEcommerce.Api.Tests.Services;

public class MockProductServiceTests
{
    private readonly MockProductService _service = new();

    [Fact]
    public void GetAll_ReturnsAllProducts()
    {
        var products = _service.GetAll().ToList();

        Assert.NotEmpty(products);
    }

    [Fact]
    public void GetById_WithValidId_ReturnsProduct()
    {
        var product = _service.GetById(1);

        Assert.NotNull(product);
        Assert.Equal(1, product.Id);
    }

    [Fact]
    public void GetById_WithInvalidId_ReturnsNull()
    {
        var product = _service.GetById(9999);

        Assert.Null(product);
    }

    [Fact]
    public void GetAll_ProductsHaveRequiredFields()
    {
        var products = _service.GetAll().ToList();

        foreach (var product in products)
        {
            Assert.True(product.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(product.Name));
            Assert.True(product.Price > 0);
            Assert.False(string.IsNullOrWhiteSpace(product.Category));
        }
    }
}
