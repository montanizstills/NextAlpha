using NETWebAppMVC.Models;

namespace NETWebAppMVC.Services.Interfaces;

public interface IBusinessLogic
{
    public string Print();

    /// <summary>Placeholder response for GET api/products/{id}: id 0 (or omitted) returns <see cref="Print"/>.</summary>
    public string DescribeProduct(int id);

    public IReadOnlyList<Product> GetAllProducts();

    public Product? GetProductById(int id);
}
