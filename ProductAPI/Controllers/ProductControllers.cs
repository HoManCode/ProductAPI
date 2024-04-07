using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using ProductAPI.Repository;

namespace ProductAPI.Controllers;

[Route("api/products")]
[ApiController]
public class ProductControllers : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductControllers(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpPost]
    public IActionResult CreateProduct([FromBody]Product product)
    {
        _productRepository.CreateProduct(product);
        return Ok(product.Id);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetProductById(int id)
    {
        return Ok(_productRepository.GetProductById(id));
    }
}