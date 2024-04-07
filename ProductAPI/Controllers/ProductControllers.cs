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
        try
        {
            var existingProduct = _productRepository.GetByNameAndBrand(product.Name, product.Brand);
            if (existingProduct != null)
            {
                return Conflict("There is already a product with \"the same name and brand\"."); // HTTP 409 Conflict
            }
            _productRepository.Create(product);
            return Ok(product);
        }
        catch (ArgumentNullException ex)
        {
            return NotFound(ex);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("{id}")]
    public IActionResult GetProductById(int id)
    {
        Product? product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound("Product does not exist");
        }
        return Ok(product);
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateProduct([FromBody]Product product, int id)
    {
        try
        {
            var existingProduct = _productRepository.GetByNameAndBrand(product.Name, product.Brand);
            if (existingProduct != null)
            {
                return Conflict("There is already a product with \"the same name and brand\"."); // HTTP 409 Conflict
            }
            _productRepository.Update(product,id);
            return Ok(product);
        }
        catch (ArgumentNullException ex)
        {
            return NotFound("Product does not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        try
        {
            _productRepository.Delete(id);
            return Ok();
        }
        catch (ArgumentException)
        {
            return NotFound("Product does not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}