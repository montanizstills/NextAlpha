using NETWebAppMVC.Models;
using NETWebAppMVC.Repositories.Interfaces;
using NETWebAppMVC.Services.Interfaces;

namespace NETWebAppMVC.Services;

public class BusinessLogic : IBusinessLogic
{
    private readonly IProductRepository _productRepository;

    public BusinessLogic(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public string Print() => "My Products!!";

    public string DescribeProduct(int id) => id == 0 ? Print() : "Goodbye World!";

    public IReadOnlyList<Product> GetAllProducts() => _productRepository.GetAllProducts();

    public Product? GetProductById(int id) => _productRepository.GetAllProducts().FirstOrDefault(p => p.Id == id);
}
