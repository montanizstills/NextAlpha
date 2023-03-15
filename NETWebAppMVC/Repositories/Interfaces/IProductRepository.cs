using NETWebAppMVC.Models;

namespace NETWebAppMVC.Repositories.Interfaces;

public interface IProductRepository
{
    public List<Product> GetAllProducts();
}