using Microsoft.AspNetCore.Mvc;
using NETWebAppMVC.Models;
using NETWebAppMVC.Services.Interfaces;

namespace NETWebAppMVC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IBusinessLogic _businessLogic;

    public ProductsController(IBusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
    }

    [HttpGet("all")]
    public string GetProducts()
    {
        return _businessLogic.Print();
    }

    [HttpGet("{id:int?}")]
    public string GetProductById(int id)
    {
        return _businessLogic.DescribeProduct(id);
    }

    /// <summary>
    /// Legacy POST form of <see cref="GetProductById"/>, kept so existing callers keep working.
    /// New callers should use GET.
    /// </summary>
    [HttpPost("{id:int?}")]
    [Obsolete("Use GET api/products/{id} instead.")]
    public string GetProductByIdLegacyPost(int id)
    {
        return _businessLogic.DescribeProduct(id);
    }

    [HttpPost("new")]
    public Product NewProduct([FromBody] Product product)
    {
        return product;
    }
}
