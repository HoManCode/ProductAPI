using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;

namespace ProductAPI.Repository;

public interface IProductRepository
{
    public Task Create(Product product);
    
    public Task<Product?> GetById(int id);
    
    public Task<List<Product>> GetAll(QueryParameters queryParameters);

    public Task<Product?> GetByNameAndBrand(string name, string brand);
    
    public Task Update(Product product, int id);
    
    public Task Delete(int id);
    
}