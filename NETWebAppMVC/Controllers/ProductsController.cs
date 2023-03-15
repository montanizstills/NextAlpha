using Microsoft.AspNetCore.Mvc;
using NETWebAppMVC.Models;
using NETWebAppMVC.Services;

namespace NETWebAppMVC.Controllers;

[Controller]
[Route("api/[controller]")]
public class ProductsController
{
    private readonly BusinessLogic _myService = new BusinessLogic();

    [HttpGet("all")]
    public string GetProducts()
    {
        return _myService.Print();
    }

    [HttpPost("{id:int?}")]
    public string GetProductById(int id)
    {
        return id == 0 ? _myService.Print() : "Goodbye World!";
    }

    [HttpPost("new")]
    public Product NewProduct([FromBody] Product product)
    {
        return product;
    }
}