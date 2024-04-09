using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductAPI.Controllers;
using ProductAPI.Models;
using ProductAPI.Repository;
using Xunit;
using Moq;

namespace ProductAPITest.Tests;


public class ProductControllerTests
{
    private readonly ProductController _sut;
    private readonly Mock<ILogger<ProductController>> _mockLogger = new();
    private readonly Mock<IProductRepository> _mockProductRepository = new();
    
    public ProductControllerTests()
    {
        _sut = new ProductController(_mockProductRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateProduct_ProductDoesNotExist_ReturnsOkResult()
    {
        // Arrange
        var product = new Product { Name = "Test Product", Brand = "Test Brand" };
        _mockProductRepository.Setup(repo => repo.GetByNameAndBrand(product.Name, product.Brand)).ReturnsAsync((Product)null);

        // Act
        var result = await _sut.CreateProduct(product);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(product, okResult.Value);
    }

    [Fact]
    public async Task CreateProduct_ProductAlreadyExists_ReturnsConflictResult()
    {
        // Arrange
        var product = new Product { Name = "Existing Product", Brand = "Existing Brand" };
        _mockProductRepository.Setup(repo => repo.GetByNameAndBrand(product.Name, product.Brand)).ReturnsAsync(product);

        // Act
        var result = await _sut.CreateProduct(product);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal("There is already a product with \"the same name and brand\".", conflictResult.Value);
    }
    
    [Fact]
    public async Task GetProductById_ProductExists_ReturnsOkResult()
    {
        // Arrange
        int productId = 1;
        var expectedProduct = new Product { Id = productId, Name = "Test Product", Price = 10.99m };
        _mockProductRepository.Setup(repo => repo.GetById(productId)).ReturnsAsync(expectedProduct);

        // Act
        var result = await _sut.GetProductById(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualProduct = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(expectedProduct, actualProduct);
    }

    [Fact]
    public async Task GetProductById_ProductDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        int productId = 1;
        _mockProductRepository.Setup(repo => repo.GetById(productId)).ReturnsAsync((Product)null);

        // Act
        var result = await _sut.GetProductById(productId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Product does not exist", notFoundResult.Value);
    }
    
    [Fact]
    public async Task GetAllProducts_ReturnsOkResultWithProducts()
    {
        // Arrange
        var queryParameters = new QueryParameters();
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Brand = "Brand 1", Price = 11.1m },
            new Product { Id = 2, Name = "Product 2", Brand = "Brand 2", Price = 22.2m }
        };
        _mockProductRepository.Setup(repo => repo.GetAll(queryParameters)).ReturnsAsync(products);

        // Act
        var result = await _sut.GetAllProducts(queryParameters);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.Equal(2, returnedProducts.Count());
    }
    
    [Fact]
    public async Task UpdateProduct_ProductExists_ReturnsOkResult()
    {
        // Arrange
        int productId = 1;
        var product = new Product { Id = productId, Name = "Updated Product", Brand = "Updated Brand" };
        _mockProductRepository.Setup(repo => repo.GetByNameAndBrand(product.Name, product.Brand)).ReturnsAsync((Product)null);

        // Act
        var result = await _sut.UpdateProduct(product, productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(product, okResult.Value);
    }
    
    [Fact]
    public async Task GetAllProducts_ValidQueryParameters_ReturnsOk()
    {
        // Arrange
        var queryParameters = new QueryParameters
        {
            MinPrice = 0,
            MaxPrice = 100
        };
        _mockProductRepository.Setup(repo => repo.GetAll(queryParameters)).ReturnsAsync([]);
        
        // Act
        var result = await _sut.GetAllProducts(queryParameters);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.Empty(products);
    }
    
    [Theory]
    [InlineData(-1, 100)] // Invalid MinPrice
    [InlineData(0, -1)]   // Invalid MaxPrice
    [InlineData(100, 50)] // MinPrice greater than MaxPrice
    public async Task GetAllProducts_InvalidQueryParameters_ReturnsBadRequest(int minPrice, int maxPrice)
    {
        // Arrange
        var queryParameters = new QueryParameters
        {
            MinPrice = minPrice,
            MaxPrice = maxPrice
        };

        // Act
        var result = await _sut.GetAllProducts(queryParameters);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid price range.", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateProduct_ProductAlreadyExists_ReturnsConflictResult()
    {
        // Arrange
        int productId = 1;
        var existingProduct = new Product { Id = productId, Name = "Existing Product", Brand = "Existing Brand" };
        var updatedProduct = new Product { Id = productId, Name = "Updated Product", Brand = "Updated Brand" };
        _mockProductRepository.Setup(repo => repo.GetByNameAndBrand(updatedProduct.Name, updatedProduct.Brand)).ReturnsAsync(existingProduct);

        // Act
        var result = await _sut.UpdateProduct(updatedProduct, productId);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal("There is already a product with \"the same name and brand\".", conflictResult.Value);
    }
    
    [Fact]
    public async Task DeleteProduct_ProductExists_ReturnsOkResult()
    {
        // Arrange
        int productId = 1;

        // Act
        var result = await _sut.DeleteProduct(productId);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        _mockProductRepository.Verify(repo => repo.Delete(productId), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_ProductDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        int productId = 1;
        _mockProductRepository.Setup(repo => repo.Delete(productId)).Throws(new ArgumentException("Product does not exist"));

        // Act
        var result = await _sut.DeleteProduct(productId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Product does not exist", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteProduct_ExceptionThrown_ReturnsBadRequestResult()
    {
        // Arrange
        int productId = 1;
        string errorMessage = "Internal server error";
        _mockProductRepository.Setup(repo => repo.Delete(productId)).Throws(new Exception(errorMessage));

        // Act
        var result = await _sut.DeleteProduct(productId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errorMessage, badRequestResult.Value);
    }
    
}