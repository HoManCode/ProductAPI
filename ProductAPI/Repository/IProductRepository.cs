using ProductAPI.Models;

namespace ProductAPI.Repository;

public interface IProductRepository
{
    public void CreateProduct(Product product);
    public Product GetProductById(int id);
    
    public void UpdateProduct(Product product);
    
    public void DeleteProduct(Product product);
}