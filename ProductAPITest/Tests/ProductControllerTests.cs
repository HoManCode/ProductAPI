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
    public void CreateProduct_ProductDoesNotExist_ReturnsOkResult()
    {
        // Arrange
        var product = new Product { Name = "Test Product", Brand = "Test Brand" };
        _mockProductRepository.Setup(repo => repo.GetByNameAndBrand(product.Name, product.Brand)).Returns((Product)null);

        // Act
        var result = _sut.CreateProduct(product);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(product, okResult.Value);
    }

    [Fact]
    public void CreateProduct_ProductAlreadyExists_ReturnsConflictResult()
    {
        // Arrange
        var product = new Product { Name = "Existing Product", Brand = "Existing Brand" };
        _mockProductRepository.Setup(repo => repo.GetByNameAndBrand(product.Name, product.Brand)).Returns(product);

        // Act
        var result = _sut.CreateProduct(product);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal("There is already a product with \"the same name and brand\".", conflictResult.Value);
    }
    
    [Fact]
    public void GetProductById_ProductExists_ReturnsOkResult()
    {
        // Arrange
        int productId = 1;
        var expectedProduct = new Product { Id = productId, Name = "Test Product", Price = 10.99m };
        _mockProductRepository.Setup(repo => repo.GetById(productId)).Returns(expectedProduct);

        // Act
        var result = _sut.GetProductById(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualProduct = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(expectedProduct, actualProduct);
    }

    [Fact]
    public void GetProductById_ProductDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        int productId = 1;
        _mockProductRepository.Setup(repo => repo.GetById(productId)).Returns((Product)null);

        // Act
        var result = _sut.GetProductById(productId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Product does not exist", notFoundResult.Value);
    }
    
    [Fact]
    public void UpdateProduct_ProductExists_ReturnsOkResult()
    {
        // Arrange
        int productId = 1;
        var product = new Product { Id = productId, Name = "Updated Product", Brand = "Updated Brand" };
        _mockProductRepository.Setup(repo => repo.GetByNameAndBrand(product.Name, product.Brand)).Returns((Product)null);

        // Act
        var result = _sut.UpdateProduct(product, productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(product, okResult.Value);
    }

    [Fact]
    public void UpdateProduct_ProductAlreadyExists_ReturnsConflictResult()
    {
        // Arrange
        int productId = 1;
        var existingProduct = new Product { Id = productId, Name = "Existing Product", Brand = "Existing Brand" };
        var updatedProduct = new Product { Id = productId, Name = "Updated Product", Brand = "Updated Brand" };
        _mockProductRepository.Setup(repo => repo.GetByNameAndBrand(updatedProduct.Name, updatedProduct.Brand)).Returns(existingProduct);

        // Act
        var result = _sut.UpdateProduct(updatedProduct, productId);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal("There is already a product with \"the same name and brand\".", conflictResult.Value);
    }
    
    [Fact]
        public void DeleteProduct_ProductExists_ReturnsOkResult()
        {
            // Arrange
            int productId = 1;

            // Act
            var result = _sut.DeleteProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _mockProductRepository.Verify(repo => repo.Delete(productId), Times.Once);
        }

        [Fact]
        public void DeleteProduct_ProductDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            int productId = 1;
            _mockProductRepository.Setup(repo => repo.Delete(productId)).Throws(new ArgumentException("Product does not exist"));

            // Act
            var result = _sut.DeleteProduct(productId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Product does not exist", notFoundResult.Value);
        }

        [Fact]
        public void DeleteProduct_ExceptionThrown_ReturnsBadRequestResult()
        {
            // Arrange
            int productId = 1;
            string errorMessage = "Internal server error";
            _mockProductRepository.Setup(repo => repo.Delete(productId)).Throws(new Exception(errorMessage));

            // Act
            var result = _sut.DeleteProduct(productId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errorMessage, badRequestResult.Value);
        }
    
}