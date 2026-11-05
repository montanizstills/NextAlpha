using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NETWebAppMVC.Models;
using NETWebAppMVC.Services.Interfaces;
using Xunit;

namespace NextAlpha.Application.Tests;

public class ApiEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetAll_ReturnsPlaceholderText()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/products/all");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("My Products!!", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("/api/products", "My Products!!")]
    [InlineData("/api/products/0", "My Products!!")]
    [InlineData("/api/products/42", "Goodbye World!")]
    public async Task GetById_IsAGet(string url, string expected)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expected, await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("/api/products/0", "My Products!!")]
    [InlineData("/api/products/42", "Goodbye World!")]
    public async Task GetById_LegacyPostStillWorks(string url, string expected)
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsync(url, content: null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expected, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task New_EchoesProduct()
    {
        var client = _factory.CreateClient();
        var product = new Product
        {
            Id = 3,
            Name = "Pear",
            Price = 1.25,
            IsAvailable = true,
            Category = new Category { Id = 1, Name = "Fruit" },
        };

        var response = await client.PostAsJsonAsync("/api/products/new", product);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var echoed = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(echoed);
        Assert.Equal(3, echoed!.Id);
        Assert.Equal("Pear", echoed.Name);
        Assert.Equal("Fruit", echoed.Category?.Name);
    }

    [Fact]
    public async Task New_AcceptsPartialBody()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/products/new", new { id = 5 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var echoed = await response.Content.ReadFromJsonAsync<Product>();
        Assert.Equal(5, echoed!.Id);
        Assert.Null(echoed.Category);
    }

    [Fact]
    public async Task UnknownRoute_ReturnsProblemDetails()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("/api/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task UnhandledException_InProduction_ReturnsProblemDetails500()
    {
        var client = _factory.WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Production");
            b.ConfigureTestServices(s => s.AddScoped<IBusinessLogic, ThrowingBusinessLogic>());
        }).CreateClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("/api/products/all");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.DoesNotContain("boom", await response.Content.ReadAsStringAsync());
    }

    private sealed class ThrowingBusinessLogic : IBusinessLogic
    {
        public string Print() => throw new InvalidOperationException("boom");
        public string DescribeProduct(int id) => throw new InvalidOperationException("boom");
        public IReadOnlyList<Product> GetAllProducts() => throw new InvalidOperationException("boom");
        public Product? GetProductById(int id) => throw new InvalidOperationException("boom");
    }

    [Fact]
    public async Task Swagger_IsServedInDevelopment()
    {
        var client = _factory.WithWebHostBuilder(b => b.UseEnvironment("Development")).CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("/api/Products/all", json);
    }

    [Fact]
    public async Task Swagger_IsNotServedInProduction()
    {
        var client = _factory.WithWebHostBuilder(b => b.UseEnvironment("Production")).CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
