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
    
    public void CreateProduct(Product product)
    {
        _context.Set<Product>().Add(product);
        _context.SaveChanges();
    }

    public Product GetProductById(int id)
    {
        return _context.Products.SingleOrDefault(product => product.Id == id);
    }

    public void UpdateProduct(Product product)
    {
        throw new NotImplementedException();
    }

    public void DeleteProduct(Product product)
    {
        throw new NotImplementedException();
    }
}