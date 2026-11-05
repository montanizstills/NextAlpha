using NETWebAppMVC.Models;
using NETWebAppMVC.Repositories.Interfaces;

namespace NETWebAppMVC.Repositories;

public class ProductRepository : IProductRepository
{
    public List<Product> GetAllProducts()
    {
        return new List<Product>()
        {
            new Product()
            {
                Id = 1, Category = new Category() { Id = 0, Name = "Fruit" }, IsAvailable = true, Name = "Apple",
                Price = 1.00
            },
            new Product()
            {
                Id = 2, Category = new Category() { Id = 0, Name = "Vegetable" }, IsAvailable = false, Name = "Carrot",
                Price = 0.75
            }
        };
    }
}