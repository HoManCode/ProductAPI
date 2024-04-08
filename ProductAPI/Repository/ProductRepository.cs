using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Repository;

public class ProductRepository : IProductRepository
{
    private readonly ProductContext _context;

    public ProductRepository(ProductContext context)
    {
        _context = context;
    }
    
    public async Task Create(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task<Product?> GetById(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<List<Product>> GetAll(QueryParameters queryParameters)
    {
        IQueryable<Product> products = _context.Products;
        
        if (queryParameters.MinPrice != null)
        {
            products = products.Where(
                p => p.Price >= queryParameters.MinPrice);
        }
        
        if (queryParameters.MaxPrice != null)
        {
            products = products.Where(
                p => p.Price <= queryParameters.MaxPrice);
        }
        
        products = products
            .Skip(queryParameters.Size * (queryParameters.Page - 1))
            .Take(queryParameters.Size);
        
        return await products.ToListAsync();
    }

    public async Task<Product?> GetByNameAndBrand(string name, string brand)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Name == name && p.Brand == brand);
    }

    public async Task Update(Product product, int id)
    {
        var existingProduct = await _context.Products.FindAsync(id);

        if (existingProduct != null)
        {
            existingProduct.Id = product.Id;
            existingProduct.Name = product.Name;
            existingProduct.Brand = product.Brand;
            existingProduct.Price = product.Price;
            _context.Products.Add(existingProduct);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentException($"Product does not exist");
        }
    }

    public async Task Delete(int id)
    {
        var existingProduct = await _context.Products.FindAsync(id);
        
        if (existingProduct != null)
        {
            _context.Set<Product>().Remove(existingProduct);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentException($"Product does not exist");
        }
    }
}