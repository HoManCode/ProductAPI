using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using ProductAPI.Repository;

namespace ProductAPI.Controllers;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductRepository productRepository, ILogger<ProductController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    [HttpPost]
    public IActionResult CreateProduct([FromBody]Product product)
    {
        _logger.LogInformation("creating product with Name: {ProductName} and Brand: {ProductBrand}", product.Name,product.Brand);
        try
        {
            var existingProduct = _productRepository.GetByNameAndBrand(product.Name, product.Brand);
            if (existingProduct != null)
            {
                _logger.LogError("There is already a product with Name: {ProductName} and Brand: {ProductBrand}", product.Name,product.Brand);
                return Conflict("There is already a product with \"the same name and brand\"."); // HTTP 409 Conflict
            }
            _productRepository.Create(product);
            return Ok(product);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request: {ErrorMessage}", ex.Message);
            return NotFound(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request: {ErrorMessage}", ex.Message);
            return BadRequest(ex);
        }
    }
    
    [HttpGet("{id}")]
    public IActionResult GetProductById(int id)
    {
        _logger.LogInformation("Getting product with Id: {ProductId}", id);
        var product = _productRepository.GetById(id);
        if (product != null) return Ok(product);
        _logger.LogInformation("Product with Id: {ProductId} does not exist", id);
        return NotFound("Product does not exist");
    }
    
    [HttpGet]
    public IActionResult GetAllProducts([FromQuery]QueryParameters queryParameters)
    {
        _logger.LogInformation("Getting requested products");
        IQueryable<Product> products = _productRepository.GetAll(queryParameters);
        return Ok(products.ToList());
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateProduct([FromBody]Product product, int id)
    {
        _logger.LogInformation("Updating product with Id: {ProductId}", id);
        try
        {
            var existingProduct = _productRepository.GetByNameAndBrand(product.Name, product.Brand);
            if (existingProduct != null)
            {
                _logger.LogError("There is already a product with Name: {ProductName} and Brand: {ProductBrand}", product.Name,product.Brand);
                return Conflict("There is already a product with \"the same name and brand\"."); // HTTP 409 Conflict
            }
            _productRepository.Update(product,id);
            return Ok(product);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request: {ErrorMessage}", ex.Message);
            return NotFound("Product does not exist");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request: {ErrorMessage}", ex.Message);
            return BadRequest(ex);
        }
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        _logger.LogInformation("Deleting product with Id: {ProductId}", id);
        try
        {
            _productRepository.Delete(id);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request: {ErrorMessage}", ex.Message);
            return NotFound("Product does not exist");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request: {ErrorMessage}", ex.Message);
            return BadRequest(ex.Message);
        }
    }
}