using ProductAPI.Models;

namespace ProductAPI.Repository;

public interface IProductRepository
{
    public void Create(Product product);
    
    public Product? GetById(int id);

    public Product? GetByNameAndBrand(string name, string brand);
    
    public void Update(Product product, int id);
    
    public void Delete(int id);
    
}