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
    public async Task<IActionResult> CreateProduct([FromBody]Product product)
    {
        _logger.LogInformation("creating product with Name: {ProductName} and Brand: {ProductBrand}", product.Name,product.Brand);
        try
        {
            var existingProduct = await _productRepository.GetByNameAndBrand(product.Name, product.Brand);
            if (existingProduct != null)
            {
                _logger.LogError("There is already a product with Name: {ProductName} and Brand: {ProductBrand}", product.Name,product.Brand);
                return Conflict("There is already a product with \"the same name and brand\"."); 
            }
            await _productRepository.Create(product);
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
    public async Task<IActionResult> GetProductById(int id)
    {
        _logger.LogInformation("Getting product with Id: {ProductId}", id);
        // Validate parameter
        if (id <= 0 )
        {
            return BadRequest("Invalid id");
        }
        var product = await _productRepository.GetById(id);
        if (product != null) return Ok(product);
        _logger.LogInformation("Product with Id: {ProductId} does not exist", id);
        return NotFound("Product does not exist");
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery]QueryParameters queryParameters)
    {
        _logger.LogInformation("Getting requested products");
        // Validate query parameters
        if (queryParameters.MinPrice < 0 || queryParameters.MaxPrice < 0 || queryParameters.MinPrice > queryParameters.MaxPrice)
        {
            return BadRequest("Invalid price range.");
        }
        var products = await _productRepository.GetAll(queryParameters);
        return Ok(products);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct([FromBody]Product product, int id)
    {
        _logger.LogInformation("Updating product with Id: {ProductId}", id);
        try
        {
            var existingProduct = await _productRepository.GetByNameAndBrand(product.Name, product.Brand);
            if (existingProduct != null)
            {
                _logger.LogError("There is already a product with Name: {ProductName} and Brand: {ProductBrand}", product.Name,product.Brand);
                return Conflict("There is already a product with \"the same name and brand\"."); 
            }
            await _productRepository.Update(product,id);
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
    public async Task<IActionResult> DeleteProduct(int id)
    {
        _logger.LogInformation("Deleting product with Id: {ProductId}", id);
        try
        {
            await _productRepository.Delete(id);
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