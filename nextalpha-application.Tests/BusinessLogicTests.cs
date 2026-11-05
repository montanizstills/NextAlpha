using NETWebAppMVC.Models;
using NETWebAppMVC.Repositories;
using NETWebAppMVC.Repositories.Interfaces;
using NETWebAppMVC.Services;
using Xunit;

namespace NextAlpha.Application.Tests;

public class BusinessLogicTests
{
    private sealed class FakeProductRepository : IProductRepository
    {
        public List<Product> Products { get; } = new()
        {
            new Product { Id = 7, Name = "Widget", Price = 2.5, IsAvailable = true },
            new Product { Id = 9, Name = "Gadget", Price = 9.99, IsAvailable = false },
        };

        public List<Product> GetAllProducts() => Products;
    }

    [Fact]
    public void Print_ReturnsPlaceholderText()
    {
        var sut = new BusinessLogic(new FakeProductRepository());

        Assert.Equal("My Products!!", sut.Print());
    }

    [Theory]
    [InlineData(0, "My Products!!")]
    [InlineData(1, "Goodbye World!")]
    [InlineData(-5, "Goodbye World!")]
    public void DescribeProduct_MatchesLegacyBehaviour(int id, string expected)
    {
        var sut = new BusinessLogic(new FakeProductRepository());

        Assert.Equal(expected, sut.DescribeProduct(id));
    }

    [Fact]
    public void GetAllProducts_ReturnsRepositoryProducts()
    {
        var repo = new FakeProductRepository();
        var sut = new BusinessLogic(repo);

        Assert.Equal(repo.Products.Select(p => p.Id), sut.GetAllProducts().Select(p => p.Id));
    }

    [Fact]
    public void GetProductById_FindsExistingProduct()
    {
        var sut = new BusinessLogic(new FakeProductRepository());

        var product = sut.GetProductById(9);

        Assert.NotNull(product);
        Assert.Equal("Gadget", product!.Name);
    }

    [Fact]
    public void GetProductById_ReturnsNullWhenMissing()
    {
        var sut = new BusinessLogic(new FakeProductRepository());

        Assert.Null(sut.GetProductById(12345));
    }

    [Fact]
    public void ProductRepository_SeedDataIsWellFormed()
    {
        var products = new ProductRepository().GetAllProducts();

        Assert.Equal(2, products.Count);
        Assert.All(products, p =>
        {
            Assert.False(string.IsNullOrWhiteSpace(p.Name));
            Assert.NotNull(p.Category);
        });
    }
}
