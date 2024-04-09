using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;
using ProductAPI.Repository;
using Xunit;

namespace ProductAPITest.Tests;

public class ProductRepositoryTests
{
    private static readonly DbContextOptions<ProductContext> _options = new DbContextOptionsBuilder<ProductContext>()
        .UseInMemoryDatabase(databaseName: "Test_Product_Database")
        .Options;
    private readonly ProductContext _context = new (_options);
    
    [Fact]
    public async Task GetById_ValidId_ReturnsProduct()
    {
        // Arrange
        var product = new Product { Id = 13, Name = "Test Product", Brand = "Test Brand", Price = 10.1m };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        var repository = new ProductRepository(_context);

        // Act
        var result = await repository.GetById(product.Id);

        // Assert
        Assert.Equal(product.Id, result?.Id);
        Assert.Equal(product.Name, result?.Name);
        Assert.Equal(product.Brand, result?.Brand);
        Assert.Equal(product.Price, result?.Price);
    }

    [Fact]
    public async Task GetById_InvalidId_ReturnsNull()
    {
        // Arrange
        var repository = new ProductRepository(_context);

        // Act
        var result = await repository.GetById(100); // Assuming Id 100 does not exist

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task Create_Product_ValidlyAdded()
    {
        // Arrange
        var repository = new ProductRepository(_context);

        var product = new Product { Id = 10, Name = "Test Product", Brand = "Test Brand", Price = 10.9m };

        // Act
        await repository.Create(product);

        // Assert
        var createdProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        Assert.NotNull(createdProduct);
        Assert.Equal(product.Name, createdProduct.Name);
        Assert.Equal(product.Brand, createdProduct.Brand);
        Assert.Equal(product.Price, createdProduct.Price);
    }
    
    [Fact]
    public async Task GetAll_ReturnsFilteredProducts()
    {
        // Arrange
        var repository = new ProductRepository(_context);
        _context.Products.AddRange(new List<Product>
        {
            new Product { Id = 100, Name = "Product 1", Brand = "Test Brand", Price = 100 },
            new Product { Id = 200, Name = "Product 2", Brand = "Test Brand", Price = 200 },
            new Product { Id = 300, Name = "Product 3", Brand = "Test Brand", Price = 300 }
        });
        await _context.SaveChangesAsync();
        var queryParameters = new QueryParameters { MinPrice = 150, MaxPrice = 250, Page = 1, Size = 10 };

        // Act
        var result = await repository.GetAll(queryParameters);

        // Assert
        Assert.Single(result);
        Assert.Equal(200, result.First().Id); // Product with price between 15 and 25
    }
    
    [Fact]
    public async Task GetByNameAndBrand_ValidNameAndBrand_ReturnsProduct()
    {
        // Arrange
        var repository = new ProductRepository(_context);

        var product = new Product { Id = 1, Name = "Test Product", Brand = "Test Brand", Price = 10.0m };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await repository.GetByNameAndBrand(product.Name, product.Brand);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result?.Id);
        Assert.Equal(product.Name, result?.Name);
        Assert.Equal(product.Brand, result?.Brand);
        Assert.Equal(product.Price, result?.Price);
    }

    [Fact]
    public async Task GetByNameAndBrand_InvalidNameOrBrand_ReturnsNull()
    {
        // Arrange
        var repository = new ProductRepository(_context);

        // Act
        var result = await repository.GetByNameAndBrand("Nonexistent Name", "Nonexistent Brand");

        // Assert
        Assert.Null(result);
    }
    
            [Fact]
        public async Task Update_ExistingProduct_SuccessfullyUpdated()
        {
            // Arrange
            var repository = new ProductRepository(_context);

            var existingProduct = new Product { Id = 89, Name = "Existing Product", Brand = "Existing Brand", Price = 10.0m };
            _context.Products.Add(existingProduct);
            await _context.SaveChangesAsync();

            var updatedProduct = new Product { Id = 90, Name = "Updated Product", Brand = "Updated Brand", Price = 20.0m };

            // Act
            await repository.Update(updatedProduct, existingProduct.Id);

            // Assert
            var updatedEntity = await _context.Products.FindAsync(existingProduct.Id);
            Assert.NotNull(updatedEntity);
            Assert.Equal(updatedProduct.Name, updatedEntity.Name);
            Assert.Equal(updatedProduct.Brand, updatedEntity.Brand);
            Assert.Equal(updatedProduct.Price, updatedEntity.Price);
        }

        [Fact]
        public async Task Update_NonExistingProduct_ThrowsException()
        {
            // Arrange
            var repository = new ProductRepository(_context);

            var nonExistingProductId = 10000; // Assuming this ID does not exist in the database
            var updatedProduct = new Product { Id = nonExistingProductId, Name = "Updated Product", Brand = "Updated Brand", Price = 20.0m };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => repository.Update(updatedProduct, nonExistingProductId));
        }
        
        [Fact]
        public async Task Delete_ExistingProduct_SuccessfullyDeleted()
        {
            // Arrange
            var repository = new ProductRepository(_context);

            var existingProduct = new Product { Id = 83, Name = "Existing Product", Brand = "Existing Brand", Price = 50.0m };
            _context.Products.Add(existingProduct);
            await _context.SaveChangesAsync();

            // Act
            await repository.Delete(existingProduct.Id);

            // Assert
            var deletedEntity = await _context.Products.FindAsync(existingProduct.Id);
            Assert.Null(deletedEntity);
        }

        [Fact]
        public async Task Delete_NonExistingProduct_ThrowsException()
        {
            // Arrange
            var repository = new ProductRepository(_context);

            var nonExistingProductId = 84; // Assuming this ID does not exist in the database

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => repository.Delete(nonExistingProductId));
        }
    
    

}