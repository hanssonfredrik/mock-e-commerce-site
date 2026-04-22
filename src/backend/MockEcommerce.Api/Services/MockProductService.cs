using MockEcommerce.Api.Models;

namespace MockEcommerce.Api.Services;

public class MockProductService : IProductService
{
    private static readonly List<Product> Products =
    [
        new Product
        {
            Id = 1,
            Name = "Wireless Headphones",
            Description = "Over-ear noise-cancelling wireless headphones with 30-hour battery life.",
            Price = 79.99m,
            Category = "Electronics",
            Stock = 25,
            ImageUrl = "https://placehold.co/300x300?text=Headphones"
        },
        new Product
        {
            Id = 2,
            Name = "Running Shoes",
            Description = "Lightweight breathable running shoes for all-terrain use.",
            Price = 59.99m,
            Category = "Footwear",
            Stock = 40,
            ImageUrl = "https://placehold.co/300x300?text=Running+Shoes"
        },
        new Product
        {
            Id = 3,
            Name = "Stainless Steel Water Bottle",
            Description = "Insulated 32 oz water bottle that keeps drinks cold for 24 hours.",
            Price = 24.99m,
            Category = "Accessories",
            Stock = 100,
            ImageUrl = "https://placehold.co/300x300?text=Water+Bottle"
        },
        new Product
        {
            Id = 4,
            Name = "Mechanical Keyboard",
            Description = "Compact tenkeyless mechanical keyboard with Cherry MX Blue switches.",
            Price = 109.99m,
            Category = "Electronics",
            Stock = 15,
            ImageUrl = "https://placehold.co/300x300?text=Keyboard"
        },
        new Product
        {
            Id = 5,
            Name = "Yoga Mat",
            Description = "Non-slip 6mm thick yoga mat with carrying strap.",
            Price = 34.99m,
            Category = "Sports",
            Stock = 60,
            ImageUrl = "https://placehold.co/300x300?text=Yoga+Mat"
        }
    ];

    public IEnumerable<Product> GetAll() => Products;

    public Product? GetById(int id) => Products.FirstOrDefault(p => p.Id == id);
}
