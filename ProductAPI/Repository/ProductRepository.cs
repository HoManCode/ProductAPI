using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Repository;

public class ProductRepository : IProductRepository
{
    private ProductContext _context;

    public ProductRepository(ProductContext context)
    {
        _context = context;
    }
    
    public void Create(Product product)
    {
        _context.Set<Product>().Add(product);
        _context.SaveChanges();
    }

    public Product? GetById(int id)
    {
        return _context.Products.Find(id);
    }
    
    public Product? GetByNameAndBrand(string name, string brand)
    {
        return _context.Products.FirstOrDefault(p => p.Name == name && p.Brand == brand);
    }

    public void Update(Product product, int id)
    {
        var existingProduct = _context.Products.Find(id);
        
        existingProduct.Id = product.Id;
        existingProduct.Name = product.Name;
        existingProduct.Brand = product.Brand;
        existingProduct.Price = product.Price;
        
        _context.Set<Product>().Add(existingProduct);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var existingProduct = _context.Products.Find(id);
        _context.Set<Product>().Remove(existingProduct);
        _context.SaveChanges();
    }
}